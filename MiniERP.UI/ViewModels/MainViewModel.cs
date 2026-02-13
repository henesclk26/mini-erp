using System.Windows.Input;
using MiniERP.UI.Helpers;

namespace MiniERP.UI.ViewModels;

public class MainViewModel : BaseViewModel
{
    private BaseViewModel _currentView = null!;
    public BaseViewModel CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }

    private string _currentViewTitle = "Dashboard";
    public string CurrentViewTitle
    {
        get => _currentViewTitle;
        set => SetProperty(ref _currentViewTitle, value);
    }

    // ViewModels
    public DashboardViewModel DashboardVM { get; }
    public ProductListViewModel ProductListVM { get; }
    public CategoryViewModel CategoryVM { get; }
    public StockEntryViewModel StockEntryVM { get; }
    public StockExitViewModel StockExitVM { get; }
    public ReportViewModel ReportVM { get; }

    // Commands
    public ICommand ShowDashboardCommand { get; }
    public ICommand ShowProductsCommand { get; }
    public ICommand ShowCategoriesCommand { get; }
    public ICommand ShowStockEntryCommand { get; }
    public ICommand ShowStockExitCommand { get; }
    public ICommand ShowReportsCommand { get; }

    public MainViewModel(
        DashboardViewModel dashboardVM,
        ProductListViewModel productListVM,
        CategoryViewModel categoryVM,
        StockEntryViewModel stockEntryVM,
        StockExitViewModel stockExitVM,
        ReportViewModel reportVM)
    {
        DashboardVM = dashboardVM;
        ProductListVM = productListVM;
        CategoryVM = categoryVM;
        StockEntryVM = stockEntryVM;
        StockExitVM = stockExitVM;
        ReportVM = reportVM;

        ShowDashboardCommand = new AsyncRelayCommand(async () => await NavigateTo(DashboardVM, "Dashboard"));
        ShowProductsCommand = new AsyncRelayCommand(async () => await NavigateTo(ProductListVM, "Ürün Yönetimi"));
        ShowCategoriesCommand = new AsyncRelayCommand(async () => await NavigateTo(CategoryVM, "Kategori Yönetimi"));
        ShowStockEntryCommand = new AsyncRelayCommand(async () => await NavigateTo(StockEntryVM, "Stok Giriş"));
        ShowStockExitCommand = new AsyncRelayCommand(async () => await NavigateTo(StockExitVM, "Stok Çıkış"));
        ShowReportsCommand = new AsyncRelayCommand(async () => await NavigateTo(ReportVM, "Raporlar"));

        // Başlangıçta Dashboard göster
        _ = NavigateTo(DashboardVM, "Dashboard");
    }

    private async Task NavigateTo(BaseViewModel viewModel, string title)
    {
        CurrentView = viewModel;
        CurrentViewTitle = title;

        if (viewModel is ILoadableViewModel loadable)
        {
            await loadable.LoadDataAsync();
        }
    }
}

public interface ILoadableViewModel
{
    Task LoadDataAsync();
}
