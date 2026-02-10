using Microsoft.EntityFrameworkCore;
using MiniERP.DAL.Context;
using MiniERP.DAL.Entities;

namespace MiniERP.DAL.Repositories;

public class CategoryRepository : Repository<Category>
{
    public CategoryRepository(AppDbContext context) : base(context) { }

    public override async Task<List<Category>> GetAllAsync()
    {
        return await _dbSet
            .Include(c => c.Products)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<bool> HasProductsAsync(int categoryId)
    {
        return await _context.Products.AnyAsync(p => p.CategoryId == categoryId);
    }
}
