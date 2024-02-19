using EcommerceDotNetCore.Data;
using EcommerceDotNetCore.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDotNetCore.Repository
{
    public class CategoryRepository(ApplicationDbContext context) : IRepository<Category>
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Category> FindByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.Where(ca => ca.ParentCategory == null).Include(c => c.ChildCategories).ToListAsync();
        }
        public async Task AddAsync(Category entity)
        {
            await _context.Categories.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category entity)
        {
            _context.Categories.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Category entity)
        {
            _context.Categories.Remove(entity);
            await _context.SaveChangesAsync();
        }

    }
}
