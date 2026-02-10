using Microsoft.EntityFrameworkCore;
using MiniERP.DAL.Context;
using MiniERP.DAL.Entities;

namespace MiniERP.DAL.Repositories;

public class StockMovementRepository : Repository<StockMovement>
{
    public StockMovementRepository(AppDbContext context) : base(context) { }

    public override async Task<List<StockMovement>> GetAllAsync()
    {
        return await _dbSet
            .Include(s => s.Product)
            .OrderByDescending(s => s.MovementDate)
            .ToListAsync();
    }

    public async Task<List<StockMovement>> GetByProductIdAsync(int productId)
    {
        return await _dbSet
            .Include(s => s.Product)
            .Where(s => s.ProductId == productId)
            .OrderByDescending(s => s.MovementDate)
            .ToListAsync();
    }

    public async Task<List<StockMovement>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(s => s.Product)
            .Where(s => s.MovementDate >= startDate && s.MovementDate <= endDate)
            .OrderByDescending(s => s.MovementDate)
            .ToListAsync();
    }

    public async Task<List<StockMovement>> GetByTypeAsync(MovementType type)
    {
        return await _dbSet
            .Include(s => s.Product)
            .Where(s => s.MovementType == type)
            .OrderByDescending(s => s.MovementDate)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalEntryValueAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _dbSet.Where(s => s.MovementType == MovementType.Entry);
        if (startDate.HasValue) query = query.Where(s => s.MovementDate >= startDate.Value);
        if (endDate.HasValue) query = query.Where(s => s.MovementDate <= endDate.Value);
        return await query.SumAsync(s => s.Quantity * s.UnitPrice);
    }

    public async Task<decimal> GetTotalExitValueAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _dbSet.Where(s => s.MovementType == MovementType.Exit);
        if (startDate.HasValue) query = query.Where(s => s.MovementDate >= startDate.Value);
        if (endDate.HasValue) query = query.Where(s => s.MovementDate <= endDate.Value);
        return await query.SumAsync(s => s.Quantity * s.UnitPrice);
    }
}
