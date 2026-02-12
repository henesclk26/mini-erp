using MiniERP.BL.DTOs;
using MiniERP.DAL.Entities;
using MiniERP.DAL.Repositories;

namespace MiniERP.BL.Services;

public class ReportService : IReportService
{
    private readonly ProductRepository _productRepository;
    private readonly CategoryRepository _categoryRepository;
    private readonly StockMovementRepository _movementRepository;

    public ReportService(
        ProductRepository productRepository,
        CategoryRepository categoryRepository,
        StockMovementRepository movementRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _movementRepository = movementRepository;
    }

    public async Task<DashboardDto> GetDashboardDataAsync()
    {
        var products = await _productRepository.GetAllAsync();
        var categories = await _categoryRepository.GetAllAsync();
        var lowStockProducts = await _productRepository.GetLowStockProductsAsync();
        var todayStart = DateTime.Today;
        var todayEnd = todayStart.AddDays(1).AddTicks(-1);
        var todayMovements = await _movementRepository.GetByDateRangeAsync(todayStart, todayEnd);

        return new DashboardDto
        {
            TotalProducts = products.Count,
            TotalCategories = categories.Count,
            LowStockCount = lowStockProducts.Count,
            TotalStockValue = products.Sum(p => p.CurrentStock * p.PurchasePrice),
            TodayEntryCount = todayMovements.Count(m => m.MovementType == MovementType.Entry),
            TodayExitCount = todayMovements.Count(m => m.MovementType == MovementType.Exit),
            LowStockProducts = lowStockProducts.Take(10).Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                CategoryName = p.Category?.Name ?? "",
                CurrentStock = p.CurrentStock,
                MinStockLevel = p.MinStockLevel,
                PurchasePrice = p.PurchasePrice,
                SalePrice = p.SalePrice
            }).ToList(),
            RecentMovements = todayMovements.Take(10).Select(m => new StockMovementDto
            {
                Id = m.Id,
                ProductName = m.Product?.Name ?? "",
                MovementType = m.MovementType == MovementType.Entry ? "Giriş" : "Çıkış",
                Quantity = m.Quantity,
                UnitPrice = m.UnitPrice,
                MovementDate = m.MovementDate
            }).ToList()
        };
    }

    public async Task<List<StockMovementDto>> GetMovementReportAsync(
        DateTime? startDate, DateTime? endDate, string? movementType, int? productId)
    {
        List<StockMovement> movements;

        if (startDate.HasValue && endDate.HasValue)
            movements = await _movementRepository.GetByDateRangeAsync(startDate.Value, endDate.Value);
        else if (productId.HasValue)
            movements = await _movementRepository.GetByProductIdAsync(productId.Value);
        else
            movements = await _movementRepository.GetAllAsync();

        // Filtrele
        if (movementType == "Giriş")
            movements = movements.Where(m => m.MovementType == MovementType.Entry).ToList();
        else if (movementType == "Çıkış")
            movements = movements.Where(m => m.MovementType == MovementType.Exit).ToList();

        if (productId.HasValue && startDate.HasValue)
            movements = movements.Where(m => m.ProductId == productId.Value).ToList();

        return movements.Select(m => new StockMovementDto
        {
            Id = m.Id,
            ProductId = m.ProductId,
            ProductName = m.Product?.Name ?? "",
            MovementType = m.MovementType == MovementType.Entry ? "Giriş" : "Çıkış",
            Quantity = m.Quantity,
            UnitPrice = m.UnitPrice,
            Description = m.Description,
            MovementDate = m.MovementDate
        }).ToList();
    }

    public async Task<List<ProductDto>> GetStockReportAsync(int? categoryId, bool? lowStockOnly)
    {
        List<Product> products;

        if (lowStockOnly == true)
            products = await _productRepository.GetLowStockProductsAsync();
        else if (categoryId.HasValue)
            products = await _productRepository.GetByCategoryAsync(categoryId.Value);
        else
            products = await _productRepository.GetAllAsync();

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Barcode = p.Barcode,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name ?? "",
            PurchasePrice = p.PurchasePrice,
            SalePrice = p.SalePrice,
            CurrentStock = p.CurrentStock,
            MinStockLevel = p.MinStockLevel,
            Description = p.Description,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        }).ToList();
    }
}
