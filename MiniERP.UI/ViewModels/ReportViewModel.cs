using System.Collections.ObjectModel;
using System.Windows.Input;
using MiniERP.BL.DTOs;
using MiniERP.BL.Services;
using MiniERP.UI.Helpers;

namespace MiniERP.UI.ViewModels;

public class ReportViewModel : BaseViewModel, ILoadableViewModel
{
    private readonly IReportService _reportService;
    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;

    public ReportViewModel(IReportService reportService, ICategoryService categoryService, IProductService productService)
    {
        _reportService = reportService;
        _categoryService = categoryService;
        _productService = productService;

        LoadMovementReportCommand = new AsyncRelayCommand(LoadMovementReportAsync);
        LoadStockReportCommand = new AsyncRelayCommand(LoadStockReportAsync);

        StartDate = DateTime.Today.AddDays(-30);
        EndDate = DateTime.Today;
    }

    // Filters
    private DateTime _startDate;
    public DateTime StartDate { get => _startDate; set => SetProperty(ref _startDate, value); }

    private DateTime _endDate;
    public DateTime EndDate { get => _endDate; set => SetProperty(ref _endDate, value); }

    private string _selectedMovementType = "Tümü";
    public string SelectedMovementType { get => _selectedMovementType; set => SetProperty(ref _selectedMovementType, value); }

    public List<string> MovementTypes { get; } = new() { "Tümü", "Giriş", "Çıkış" };

    private bool _lowStockOnly;
    public bool LowStockOnly { get => _lowStockOnly; set => SetProperty(ref _lowStockOnly, value); }

    private int _selectedCategoryId;
    public int SelectedCategoryId { get => _selectedCategoryId; set => SetProperty(ref _selectedCategoryId, value); }

    // Data
    private ObservableCollection<CategoryDto> _categories = new();
    public ObservableCollection<CategoryDto> Categories { get => _categories; set => SetProperty(ref _categories, value); }

    private ObservableCollection<StockMovementDto> _movements = new();
    public ObservableCollection<StockMovementDto> Movements { get => _movements; set => SetProperty(ref _movements, value); }

    private ObservableCollection<ProductDto> _stockReport = new();
    public ObservableCollection<ProductDto> StockReport { get => _stockReport; set => SetProperty(ref _stockReport, value); }

    // Summary
    private int _totalMovements;
    public int TotalMovements { get => _totalMovements; set => SetProperty(ref _totalMovements, value); }

    private decimal _totalEntryValue;
    public decimal TotalEntryValue { get => _totalEntryValue; set => SetProperty(ref _totalEntryValue, value); }

    private decimal _totalExitValue;
    public decimal TotalExitValue { get => _totalExitValue; set => SetProperty(ref _totalExitValue, value); }

    private decimal _totalStockValue;
    public decimal TotalStockValue { get => _totalStockValue; set => SetProperty(ref _totalStockValue, value); }

    // Commands
    public ICommand LoadMovementReportCommand { get; }
    public ICommand LoadStockReportCommand { get; }

    public async Task LoadDataAsync()
    {
        IsBusy = true;
        try
        {
            var categories = await _categoryService.GetAllAsync();
            Categories = new ObservableCollection<CategoryDto>(categories);

            await LoadMovementReportAsync();
            await LoadStockReportAsync();
        }
        finally { IsBusy = false; }
    }

    private async Task LoadMovementReportAsync()
    {
        try
        {
            string? typeFilter = SelectedMovementType == "Tümü" ? null : SelectedMovementType;
            var movements = await _reportService.GetMovementReportAsync(StartDate, EndDate.AddDays(1).AddTicks(-1), typeFilter, null);
            Movements = new ObservableCollection<StockMovementDto>(movements);
            TotalMovements = movements.Count;
            TotalEntryValue = movements.Where(m => m.MovementType == "Giriş").Sum(m => m.TotalPrice);
            TotalExitValue = movements.Where(m => m.MovementType == "Çıkış").Sum(m => m.TotalPrice);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Hareket raporu hatası: {ex.Message}";
        }
    }

    private async Task LoadStockReportAsync()
    {
        try
        {
            int? categoryId = SelectedCategoryId > 0 ? SelectedCategoryId : null;
            var report = await _reportService.GetStockReportAsync(categoryId, LowStockOnly ? true : null);
            StockReport = new ObservableCollection<ProductDto>(report);
            TotalStockValue = report.Sum(p => p.StockValue);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Stok raporu hatası: {ex.Message}";
        }
    }
}
