using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using ReactCRUDAPI.Data;
using ReactCRUDAPI.Model;

using System;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(AppDbContext context, ILogger<ProductsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            _logger.LogInformation("Fetching all products.");
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching all products.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            _logger.LogInformation("Fetching product with ID: {Id}", id);
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product with ID: {Id} not found.", id);
                return NotFound();
            }
            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching the product with ID: {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        try
        {
            _logger.LogInformation("Creating a new product.");
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Product created successfully with ID: {Id}", product.Id);
            return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the product.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Product product)
    {
        if (id != product.Id)
        {
            _logger.LogWarning("Product ID mismatch for update. ID: {Id}, Product.ID: {ProductId}", id, product.Id);
            return BadRequest("Product ID mismatch.");
        }

        _context.Entry(product).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Product with ID: {Id} updated successfully.", id);
            return NoContent();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            if (!ProductExists(id))
            {
                _logger.LogWarning("Product with ID: {Id} not found for update.", id);
                return NotFound();
            }
            _logger.LogError(ex, "Concurrency error occurred while updating product with ID: {Id}.", id);
            return StatusCode(500, "Concurrency error occurred.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating the product with ID: {Id}.", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            _logger.LogInformation("Deleting product with ID: {Id}", id);
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product with ID: {Id} not found for deletion.", id);
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Product with ID: {Id} deleted successfully.", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting the product with ID: {Id}.", id);
            return StatusCode(500, "Internal server error");
        }
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }
}
