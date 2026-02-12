using MiniERP.BL.DTOs;

namespace MiniERP.BL.Services;

public interface IStockService
{
    Task<List<StockMovementDto>> GetAllMovementsAsync();
    Task<List<StockMovementDto>> GetMovementsByProductAsync(int productId);
    Task<List<StockMovementDto>> GetMovementsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<(bool Success, string Message)> AddEntryAsync(int productId, int quantity, decimal unitPrice, string? description);
    Task<(bool Success, string Message)> AddExitAsync(int productId, int quantity, decimal unitPrice, string? description);
}
