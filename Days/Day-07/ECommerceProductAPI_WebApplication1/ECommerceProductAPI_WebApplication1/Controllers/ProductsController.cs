
using ECommerceProductAPI.Data;
using ECommerceProductAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductContext _context;

        public ProductsController(ProductContext context)
        {
            _context = context;
        }

        // GET: api/products
        // GET: api/products?category=Books
        // GET: api/products?search=pen
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(
            [FromQuery] string? category,
            [FromQuery] string? search)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p =>
                    p.Category.ToLower() == category.Trim().ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                string term = search.Trim().ToLower();

                query = query.Where(p =>
                    p.Name.ToLower().Contains(term) ||
                    (p.Description != null &&
                     p.Description.ToLower().Contains(term)));
            }

            return Ok(await query.ToListAsync());
        }

        // GET: api/products/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetProduct),
                new { id = product.Id },
                product);
        }

        // PUT: api/products/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(
            int id, Product product)
        {
            var existingProduct = await _context.Products.FindAsync(id);

            if (existingProduct == null)
                return NotFound();

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Category = product.Category;
            existingProduct.Price = product.Price;
            existingProduct.StockQuantity = product.StockQuantity;

            await _context.SaveChangesAsync();

            return Ok(existingProduct);
        }

        // DELETE: api/products/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
