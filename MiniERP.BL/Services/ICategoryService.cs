using MiniERP.BL.DTOs;

namespace MiniERP.BL.Services;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<(bool Success, string Message)> AddAsync(CategoryDto dto);
    Task<(bool Success, string Message)> UpdateAsync(CategoryDto dto);
    Task<(bool Success, string Message)> DeleteAsync(int id);
}
