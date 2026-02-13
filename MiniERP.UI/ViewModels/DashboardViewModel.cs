using MiniERP.BL.DTOs;
using MiniERP.BL.Services;

namespace MiniERP.UI.ViewModels;

public class DashboardViewModel : BaseViewModel, ILoadableViewModel
{
    private readonly IReportService _reportService;

    public DashboardViewModel(IReportService reportService)
    {
        _reportService = reportService;
    }

    private int _totalProducts;
    public int TotalProducts { get => _totalProducts; set => SetProperty(ref _totalProducts, value); }

    private int _totalCategories;
    public int TotalCategories { get => _totalCategories; set => SetProperty(ref _totalCategories, value); }

    private int _lowStockCount;
    public int LowStockCount { get => _lowStockCount; set => SetProperty(ref _lowStockCount, value); }

    private decimal _totalStockValue;
    public decimal TotalStockValue { get => _totalStockValue; set => SetProperty(ref _totalStockValue, value); }

    private int _todayEntryCount;
    public int TodayEntryCount { get => _todayEntryCount; set => SetProperty(ref _todayEntryCount, value); }

    private int _todayExitCount;
    public int TodayExitCount { get => _todayExitCount; set => SetProperty(ref _todayExitCount, value); }

    private List<ProductDto> _lowStockProducts = new();
    public List<ProductDto> LowStockProducts { get => _lowStockProducts; set => SetProperty(ref _lowStockProducts, value); }

    private List<StockMovementDto> _recentMovements = new();
    public List<StockMovementDto> RecentMovements { get => _recentMovements; set => SetProperty(ref _recentMovements, value); }

    public async Task LoadDataAsync()
    {
        IsBusy = true;
        try
        {
            var dashboard = await _reportService.GetDashboardDataAsync();
            TotalProducts = dashboard.TotalProducts;
            TotalCategories = dashboard.TotalCategories;
            LowStockCount = dashboard.LowStockCount;
            TotalStockValue = dashboard.TotalStockValue;
            TodayEntryCount = dashboard.TodayEntryCount;
            TodayExitCount = dashboard.TodayExitCount;
            LowStockProducts = dashboard.LowStockProducts;
            RecentMovements = dashboard.RecentMovements;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Hata: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
