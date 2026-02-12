using MiniERP.BL.DTOs;

namespace MiniERP.BL.Services;

public interface IReportService
{
    Task<DashboardDto> GetDashboardDataAsync();
    Task<List<StockMovementDto>> GetMovementReportAsync(DateTime? startDate, DateTime? endDate, string? movementType, int? productId);
    Task<List<ProductDto>> GetStockReportAsync(int? categoryId, bool? lowStockOnly);
}
