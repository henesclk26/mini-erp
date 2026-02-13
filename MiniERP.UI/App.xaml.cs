using System.Windows;
using MiniERP.DAL.Context;
using MiniERP.DAL.Repositories;
using MiniERP.BL.Services;
using MiniERP.UI.ViewModels;
using MiniERP.UI.Views;
using Microsoft.EntityFrameworkCore;

namespace MiniERP.UI;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Database oluştur ve migrate et
        using (var context = new AppDbContext())
        {
            await context.Database.EnsureCreatedAsync();
        }

        // Manual DI - basit tutmak için
        var context2 = new AppDbContext();
        var categoryRepo = new CategoryRepository(context2);
        var productRepo = new ProductRepository(context2);
        var movementRepo = new StockMovementRepository(context2);

        ICategoryService categoryService = new CategoryService(categoryRepo);
        IProductService productService = new ProductService(productRepo);
        IStockService stockService = new StockService(movementRepo, productRepo);
        IReportService reportService = new ReportService(productRepo, categoryRepo, movementRepo);

        var dashboardVM = new DashboardViewModel(reportService);
        var productListVM = new ProductListViewModel(productService, categoryService);
        var categoryVM = new CategoryViewModel(categoryService);
        var stockEntryVM = new StockEntryViewModel(stockService, productService);
        var stockExitVM = new StockExitViewModel(stockService, productService);
        var reportVM = new ReportViewModel(reportService, categoryService, productService);

        var mainVM = new MainViewModel(dashboardVM, productListVM, categoryVM, stockEntryVM, stockExitVM, reportVM);

        var mainWindow = new MainWindow
        {
            DataContext = mainVM
        };
        mainWindow.Show();
    }
}
