using EcommerceDotNetCore.Data;
using EcommerceDotNetCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDotNetCore.Controllers
{

        [ApiController]
        [Route("api/search")]
        public class SearchController : ControllerBase
        {
            private readonly ApplicationDbContext _context;

            public SearchController(ApplicationDbContext context)
            {
                _context = context;
            }

            [HttpGet]
            public async Task<ActionResult<IEnumerable<object>>> Search(string keyword)
            {
                var products = await _context.Products
                    .Where(p => p.Name.Contains(keyword) || p.Description.Contains(keyword))
                    .ToListAsync();

                var categories = await _context.Categories
                    .Where(c => c.Name.Contains(keyword))
                    .ToListAsync();

                var result = new
                {
                    Products = products,
                    Categories = categories
                };

                return Ok(result);
            }
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<Product>>> FilterProducts(decimal? minPrice, decimal? maxPrice, int? categoryId)
        {
            var query = _context.Products.AsQueryable();

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var filteredProducts = await query.ToListAsync();
            return Ok(filteredProducts);
        }
    }
}
    
