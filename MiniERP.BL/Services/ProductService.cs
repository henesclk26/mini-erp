using MiniERP.BL.DTOs;
using MiniERP.DAL.Entities;
using MiniERP.DAL.Repositories;

namespace MiniERP.BL.Services;

public class ProductService : IProductService
{
    private readonly ProductRepository _repository;

    public ProductService(ProductRepository repository)
    {
        _repository = repository;
    }

    private ProductDto MapToDto(Product p) => new()
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
    };

    public async Task<List<ProductDto>> GetAllAsync()
    {
        var products = await _repository.GetAllAsync();
        return products.Select(MapToDto).ToList();
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        return product == null ? null : MapToDto(product);
    }

    public async Task<List<ProductDto>> GetByCategoryAsync(int categoryId)
    {
        var products = await _repository.GetByCategoryAsync(categoryId);
        return products.Select(MapToDto).ToList();
    }

    public async Task<List<ProductDto>> GetLowStockAsync()
    {
        var products = await _repository.GetLowStockProductsAsync();
        return products.Select(MapToDto).ToList();
    }

    public async Task<List<ProductDto>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllAsync();

        var products = await _repository.SearchAsync(searchTerm);
        return products.Select(MapToDto).ToList();
    }

    public async Task<(bool Success, string Message)> AddAsync(ProductDto dto)
    {
        // Validasyonlar
        if (string.IsNullOrWhiteSpace(dto.Name))
            return (false, "Ürün adı boş olamaz.");

        if (dto.PurchasePrice < 0)
            return (false, "Alış fiyatı negatif olamaz.");

        if (dto.SalePrice < 0)
            return (false, "Satış fiyatı negatif olamaz.");

        if (dto.MinStockLevel < 0)
            return (false, "Minimum stok seviyesi negatif olamaz.");

        if (!string.IsNullOrWhiteSpace(dto.Barcode))
        {
            var existing = await _repository.GetByBarcodeAsync(dto.Barcode);
            if (existing != null)
                return (false, $"'{dto.Barcode}' barkoduna sahip başka bir ürün zaten mevcut.");
        }

        var entity = new Product
        {
            Name = dto.Name.Trim(),
            Barcode = dto.Barcode?.Trim(),
            CategoryId = dto.CategoryId,
            PurchasePrice = dto.PurchasePrice,
            SalePrice = dto.SalePrice,
            CurrentStock = dto.CurrentStock,
            MinStockLevel = dto.MinStockLevel,
            Description = dto.Description?.Trim(),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        dto.Id = entity.Id;
        return (true, "Ürün başarıyla eklendi.");
    }

    public async Task<(bool Success, string Message)> UpdateAsync(ProductDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return (false, "Ürün adı boş olamaz.");

        if (dto.PurchasePrice < 0)
            return (false, "Alış fiyatı negatif olamaz.");

        if (dto.SalePrice < 0)
            return (false, "Satış fiyatı negatif olamaz.");

        var entity = await _repository.GetByIdAsync(dto.Id);
        if (entity == null)
            return (false, "Ürün bulunamadı.");

        if (!string.IsNullOrWhiteSpace(dto.Barcode))
        {
            var existing = await _repository.GetByBarcodeAsync(dto.Barcode);
            if (existing != null && existing.Id != dto.Id)
                return (false, $"'{dto.Barcode}' barkoduna sahip başka bir ürün zaten mevcut.");
        }

        entity.Name = dto.Name.Trim();
        entity.Barcode = dto.Barcode?.Trim();
        entity.CategoryId = dto.CategoryId;
        entity.PurchasePrice = dto.PurchasePrice;
        entity.SalePrice = dto.SalePrice;
        entity.MinStockLevel = dto.MinStockLevel;
        entity.Description = dto.Description?.Trim();
        entity.UpdatedAt = DateTime.Now;

        _repository.Update(entity);
        await _repository.SaveChangesAsync();
        return (true, "Ürün başarıyla güncellendi.");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return (false, "Ürün bulunamadı.");

        _repository.Delete(entity);
        await _repository.SaveChangesAsync();
        return (true, "Ürün başarıyla silindi.");
    }
}
