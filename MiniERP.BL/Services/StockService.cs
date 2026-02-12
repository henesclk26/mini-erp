using MiniERP.BL.DTOs;
using MiniERP.DAL.Entities;
using MiniERP.DAL.Repositories;

namespace MiniERP.BL.Services;

public class StockService : IStockService
{
    private readonly StockMovementRepository _movementRepository;
    private readonly ProductRepository _productRepository;

    public StockService(StockMovementRepository movementRepository, ProductRepository productRepository)
    {
        _movementRepository = movementRepository;
        _productRepository = productRepository;
    }

    private StockMovementDto MapToDto(StockMovement s) => new()
    {
        Id = s.Id,
        ProductId = s.ProductId,
        ProductName = s.Product?.Name ?? "",
        MovementType = s.MovementType == MovementType.Entry ? "Giriş" : "Çıkış",
        Quantity = s.Quantity,
        UnitPrice = s.UnitPrice,
        Description = s.Description,
        MovementDate = s.MovementDate
    };

    public async Task<List<StockMovementDto>> GetAllMovementsAsync()
    {
        var movements = await _movementRepository.GetAllAsync();
        return movements.Select(MapToDto).ToList();
    }

    public async Task<List<StockMovementDto>> GetMovementsByProductAsync(int productId)
    {
        var movements = await _movementRepository.GetByProductIdAsync(productId);
        return movements.Select(MapToDto).ToList();
    }

    public async Task<List<StockMovementDto>> GetMovementsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var movements = await _movementRepository.GetByDateRangeAsync(startDate, endDate);
        return movements.Select(MapToDto).ToList();
    }

    public async Task<(bool Success, string Message)> AddEntryAsync(int productId, int quantity, decimal unitPrice, string? description)
    {
        if (quantity <= 0)
            return (false, "Miktar sıfırdan büyük olmalıdır.");

        if (unitPrice < 0)
            return (false, "Birim fiyat negatif olamaz.");

        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            return (false, "Ürün bulunamadı.");

        // Stok hareketi oluştur
        var movement = new StockMovement
        {
            ProductId = productId,
            MovementType = MovementType.Entry,
            Quantity = quantity,
            UnitPrice = unitPrice,
            Description = description?.Trim(),
            MovementDate = DateTime.Now
        };

        // Ürün stok güncelle
        product.CurrentStock += quantity;
        product.UpdatedAt = DateTime.Now;

        await _movementRepository.AddAsync(movement);
        _productRepository.Update(product);
        await _movementRepository.SaveChangesAsync();

        return (true, $"{product.Name} için {quantity} adet stok girişi yapıldı. Mevcut stok: {product.CurrentStock}");
    }

    public async Task<(bool Success, string Message)> AddExitAsync(int productId, int quantity, decimal unitPrice, string? description)
    {
        if (quantity <= 0)
            return (false, "Miktar sıfırdan büyük olmalıdır.");

        if (unitPrice < 0)
            return (false, "Birim fiyat negatif olamaz.");

        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            return (false, "Ürün bulunamadı.");

        if (product.CurrentStock < quantity)
            return (false, $"Yetersiz stok! Mevcut stok: {product.CurrentStock}, İstenen: {quantity}");

        // Stok hareketi oluştur
        var movement = new StockMovement
        {
            ProductId = productId,
            MovementType = MovementType.Exit,
            Quantity = quantity,
            UnitPrice = unitPrice,
            Description = description?.Trim(),
            MovementDate = DateTime.Now
        };

        // Ürün stok güncelle
        product.CurrentStock -= quantity;
        product.UpdatedAt = DateTime.Now;

        await _movementRepository.AddAsync(movement);
        _productRepository.Update(product);
        await _movementRepository.SaveChangesAsync();

        string warning = product.CurrentStock <= product.MinStockLevel
            ? $" ⚠️ Dikkat: Stok minimum seviyenin altında! ({product.CurrentStock}/{product.MinStockLevel})"
            : "";

        return (true, $"{product.Name} için {quantity} adet stok çıkışı yapıldı. Mevcut stok: {product.CurrentStock}{warning}");
    }
}
