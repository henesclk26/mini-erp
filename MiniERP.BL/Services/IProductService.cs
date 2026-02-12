using MiniERP.BL.DTOs;

namespace MiniERP.BL.Services;

public interface IProductService
{
    Task<List<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(int id);
    Task<List<ProductDto>> GetByCategoryAsync(int categoryId);
    Task<List<ProductDto>> GetLowStockAsync();
    Task<List<ProductDto>> SearchAsync(string searchTerm);
    Task<(bool Success, string Message)> AddAsync(ProductDto dto);
    Task<(bool Success, string Message)> UpdateAsync(ProductDto dto);
    Task<(bool Success, string Message)> DeleteAsync(int id);
}
