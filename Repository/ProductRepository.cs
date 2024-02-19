using EcommerceDotNetCore.Data;
using EcommerceDotNetCore.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDotNetCore.Repository
{
    public class ProductRepository(ApplicationDbContext context) : IRepository<Product>
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Product> FindByIdAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }
        public async Task AddAsync(Product entity)
        {
            await _context.Products.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product entity)
        {
            _context.Products.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product entity)
        {
            _context.Products.Remove(entity);
            await _context.SaveChangesAsync();
        }

    }
}
