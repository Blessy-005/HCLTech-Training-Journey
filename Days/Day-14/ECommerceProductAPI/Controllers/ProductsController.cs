using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.Models;

namespace ProductApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ProductsController(AppDbContext db) => _db = db;

    // GET /api/products?category=Books&search=pen
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll(
        [FromQuery] string? category, [FromQuery] string? search)
    {
        var query = _db.Products.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.Category == category.Trim());

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(p => p.Name.Contains(term) ||
                                     (p.Description != null && p.Description.Contains(term)));
        }

        return Ok(await query.OrderBy(p => p.Id).ToListAsync());
    }

    // GET /api/products/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        return product is null ? NotFound(new { message = $"Product {id} not found." }) : Ok(product);
    }

    // POST /api/products
    [HttpPost]
    public async Task<ActionResult<Product>> Create(ProductDto dto)
    {
        var name = dto.Name.Trim();
        var category = dto.Category.Trim();

        if (await _db.Products.AnyAsync(p => p.Name == name && p.Category == category))
            return Conflict(new { message = $"'{name}' already exists in category '{category}'." });

        var product = new Product
        {
            Name = name,
            Description = dto.Description?.Trim(),
            Category = category,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    // PUT /api/products/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<Product>> Update(int id, ProductDto dto)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null)
            return NotFound(new { message = $"Product {id} not found." });

        var name = dto.Name.Trim();
        var category = dto.Category.Trim();

        if (await _db.Products.AnyAsync(p => p.Id != id && p.Name == name && p.Category == category))
            return Conflict(new { message = $"'{name}' already exists in category '{category}'." });

        product.Name = name;
        product.Description = dto.Description?.Trim();
        product.Category = category;
        product.Price = dto.Price;
        product.StockQuantity = dto.StockQuantity;

        await _db.SaveChangesAsync();
        return Ok(product);
    }

    // DELETE /api/products/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null)
            return NotFound(new { message = $"Product {id} not found." });

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}