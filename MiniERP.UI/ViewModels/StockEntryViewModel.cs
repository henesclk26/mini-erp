using System.Collections.ObjectModel;
using System.Windows.Input;
using MiniERP.BL.DTOs;
using MiniERP.BL.Services;
using MiniERP.UI.Helpers;

namespace MiniERP.UI.ViewModels;

public class StockEntryViewModel : BaseViewModel, ILoadableViewModel
{
    private readonly IStockService _stockService;
    private readonly IProductService _productService;

    public StockEntryViewModel(IStockService stockService, IProductService productService)
    {
        _stockService = stockService;
        _productService = productService;

        SaveCommand = new AsyncRelayCommand(SaveEntryAsync);
        ClearCommand = new RelayCommand(ClearForm);
    }

    private ObservableCollection<ProductDto> _products = new();
    public ObservableCollection<ProductDto> Products { get => _products; set => SetProperty(ref _products, value); }

    private int _selectedProductId;
    public int SelectedProductId { get => _selectedProductId; set => SetProperty(ref _selectedProductId, value); }

    private int _quantity;
    public int Quantity { get => _quantity; set => SetProperty(ref _quantity, value); }

    private decimal _unitPrice;
    public decimal UnitPrice { get => _unitPrice; set => SetProperty(ref _unitPrice, value); }

    private string? _description;
    public string? Description { get => _description; set => SetProperty(ref _description, value); }

    private ObservableCollection<StockMovementDto> _recentEntries = new();
    public ObservableCollection<StockMovementDto> RecentEntries { get => _recentEntries; set => SetProperty(ref _recentEntries, value); }

    public ICommand SaveCommand { get; }
    public ICommand ClearCommand { get; }

    public async Task LoadDataAsync()
    {
        IsBusy = true;
        try
        {
            var products = await _productService.GetAllAsync();
            Products = new ObservableCollection<ProductDto>(products);

            var movements = await _stockService.GetAllMovementsAsync();
            RecentEntries = new ObservableCollection<StockMovementDto>(
                movements.Where(m => m.MovementType == "Giriş").Take(20));
        }
        finally { IsBusy = false; }
    }

    private async Task SaveEntryAsync()
    {
        if (SelectedProductId == 0) { StatusMessage = "Lütfen bir ürün seçin."; return; }

        var (success, message) = await _stockService.AddEntryAsync(SelectedProductId, Quantity, UnitPrice, Description);
        StatusMessage = message;
        if (success)
        {
            ClearForm();
            await LoadDataAsync();
        }
    }

    private void ClearForm()
    {
        SelectedProductId = 0;
        Quantity = 0;
        UnitPrice = 0;
        Description = null;
    }
}
