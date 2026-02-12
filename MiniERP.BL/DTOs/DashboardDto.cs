namespace MiniERP.BL.DTOs;

public class DashboardDto
{
    public int TotalProducts { get; set; }
    public int TotalCategories { get; set; }
    public int LowStockCount { get; set; }
    public decimal TotalStockValue { get; set; }
    public int TodayEntryCount { get; set; }
    public int TodayExitCount { get; set; }
    public List<ProductDto> LowStockProducts { get; set; } = new();
    public List<StockMovementDto> RecentMovements { get; set; } = new();
}
