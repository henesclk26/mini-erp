using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using MiniERP.BL.DTOs;
using MiniERP.BL.Services;
using MiniERP.UI.Helpers;

namespace MiniERP.UI.ViewModels;

public class ProductListViewModel : BaseViewModel, ILoadableViewModel
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public ProductListViewModel(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;

        AddCommand = new AsyncRelayCommand(AddProductAsync);
        UpdateCommand = new AsyncRelayCommand(UpdateProductAsync);
        DeleteCommand = new AsyncRelayCommand(DeleteProductAsync);
        ClearCommand = new RelayCommand(ClearForm);
        SearchCommand = new AsyncRelayCommand(SearchAsync);
    }

    // Collections
    private ObservableCollection<ProductDto> _products = new();
    public ObservableCollection<ProductDto> Products { get => _products; set => SetProperty(ref _products, value); }

    private ObservableCollection<CategoryDto> _categories = new();
    public ObservableCollection<CategoryDto> Categories { get => _categories; set => SetProperty(ref _categories, value); }

    // Form fields
    private int _selectedId;
    public int SelectedId { get => _selectedId; set => SetProperty(ref _selectedId, value); }

    private string _name = string.Empty;
    public string Name { get => _name; set => SetProperty(ref _name, value); }

    private string? _barcode;
    public string? Barcode { get => _barcode; set => SetProperty(ref _barcode, value); }

    private int _selectedCategoryId;
    public int SelectedCategoryId { get => _selectedCategoryId; set => SetProperty(ref _selectedCategoryId, value); }

    private decimal _purchasePrice;
    public decimal PurchasePrice { get => _purchasePrice; set => SetProperty(ref _purchasePrice, value); }

    private decimal _salePrice;
    public decimal SalePrice { get => _salePrice; set => SetProperty(ref _salePrice, value); }

    private int _currentStock;
    public int CurrentStock { get => _currentStock; set => SetProperty(ref _currentStock, value); }

    private int _minStockLevel;
    public int MinStockLevel { get => _minStockLevel; set => SetProperty(ref _minStockLevel, value); }

    private string? _description;
    public string? Description { get => _description; set => SetProperty(ref _description, value); }

    private string _searchText = string.Empty;
    public string SearchText { get => _searchText; set => SetProperty(ref _searchText, value); }

    private ProductDto? _selectedProduct;
    public ProductDto? SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            SetProperty(ref _selectedProduct, value);
            if (value != null)
            {
                SelectedId = value.Id;
                Name = value.Name;
                Barcode = value.Barcode;
                SelectedCategoryId = value.CategoryId;
                PurchasePrice = value.PurchasePrice;
                SalePrice = value.SalePrice;
                CurrentStock = value.CurrentStock;
                MinStockLevel = value.MinStockLevel;
                Description = value.Description;
            }
        }
    }

    // Commands
    public ICommand AddCommand { get; }
    public ICommand UpdateCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand SearchCommand { get; }

    public async Task LoadDataAsync()
    {
        IsBusy = true;
        try
        {
            var products = await _productService.GetAllAsync();
            Products = new ObservableCollection<ProductDto>(products);

            var categories = await _categoryService.GetAllAsync();
            Categories = new ObservableCollection<CategoryDto>(categories);
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

    private async Task SearchAsync()
    {
        IsBusy = true;
        try
        {
            var products = string.IsNullOrWhiteSpace(SearchText)
                ? await _productService.GetAllAsync()
                : await _productService.SearchAsync(SearchText);
            Products = new ObservableCollection<ProductDto>(products);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AddProductAsync()
    {
        var dto = new ProductDto
        {
            Name = Name,
            Barcode = Barcode,
            CategoryId = SelectedCategoryId,
            PurchasePrice = PurchasePrice,
            SalePrice = SalePrice,
            CurrentStock = CurrentStock,
            MinStockLevel = MinStockLevel,
            Description = Description
        };
        var (success, message) = await _productService.AddAsync(dto);
        StatusMessage = message;
        if (success) { ClearForm(); await LoadDataAsync(); }
    }

    private async Task UpdateProductAsync()
    {
        if (SelectedId == 0) { StatusMessage = "Lütfen güncellenecek ürünü seçin."; return; }
        var dto = new ProductDto
        {
            Id = SelectedId,
            Name = Name,
            Barcode = Barcode,
            CategoryId = SelectedCategoryId,
            PurchasePrice = PurchasePrice,
            SalePrice = SalePrice,
            MinStockLevel = MinStockLevel,
            Description = Description
        };
        var (success, message) = await _productService.UpdateAsync(dto);
        StatusMessage = message;
        if (success) { ClearForm(); await LoadDataAsync(); }
    }

    private async Task DeleteProductAsync()
    {
        if (SelectedId == 0) { StatusMessage = "Lütfen silinecek ürünü seçin."; return; }
        var result = MessageBox.Show("Bu ürünü silmek istediğinize emin misiniz?", "Onay",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result != MessageBoxResult.Yes) return;

        var (success, message) = await _productService.DeleteAsync(SelectedId);
        StatusMessage = message;
        if (success) { ClearForm(); await LoadDataAsync(); }
    }

    private void ClearForm()
    {
        SelectedId = 0;
        Name = string.Empty;
        Barcode = null;
        SelectedCategoryId = 0;
        PurchasePrice = 0;
        SalePrice = 0;
        CurrentStock = 0;
        MinStockLevel = 0;
        Description = null;
        SelectedProduct = null;
        SearchText = string.Empty;
    }
}
