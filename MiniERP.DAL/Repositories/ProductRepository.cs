using Microsoft.EntityFrameworkCore;
using MiniERP.DAL.Context;
using MiniERP.DAL.Entities;

namespace MiniERP.DAL.Repositories;

public class ProductRepository : Repository<Product>
{
    public ProductRepository(AppDbContext context) : base(context) { }

    public override async Task<List<Product>> GetAllAsync()
    {
        return await _dbSet
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public override async Task<Product?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.StockMovements)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Product>> GetByCategoryAsync(int categoryId)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<List<Product>> GetLowStockProductsAsync()
    {
        return await _dbSet
            .Include(p => p.Category)
            .Where(p => p.CurrentStock <= p.MinStockLevel)
            .OrderBy(p => p.CurrentStock)
            .ToListAsync();
    }

    public async Task<Product?> GetByBarcodeAsync(string barcode)
    {
        return await _dbSet
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Barcode == barcode);
    }

    public async Task<List<Product>> SearchAsync(string searchTerm)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Where(p => p.Name.Contains(searchTerm) ||
                        (p.Barcode != null && p.Barcode.Contains(searchTerm)) ||
                        (p.Description != null && p.Description.Contains(searchTerm)))
            .OrderBy(p => p.Name)
            .ToListAsync();
    }
}
