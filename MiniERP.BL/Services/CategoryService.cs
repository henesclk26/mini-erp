using MiniERP.BL.DTOs;
using MiniERP.DAL.Entities;
using MiniERP.DAL.Repositories;

namespace MiniERP.BL.Services;

public class CategoryService : ICategoryService
{
    private readonly CategoryRepository _repository;

    public CategoryService(CategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var categories = await _repository.GetAllAsync();
        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            ProductCount = c.Products?.Count ?? 0
        }).ToList();
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null) return null;

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ProductCount = category.Products?.Count ?? 0
        };
    }

    public async Task<(bool Success, string Message)> AddAsync(CategoryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return (false, "Kategori adı boş olamaz.");

        var existing = await _repository.GetByNameAsync(dto.Name);
        if (existing != null)
            return (false, $"'{dto.Name}' adında bir kategori zaten mevcut.");

        var entity = new Category
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim()
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        dto.Id = entity.Id;
        return (true, "Kategori başarıyla eklendi.");
    }

    public async Task<(bool Success, string Message)> UpdateAsync(CategoryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return (false, "Kategori adı boş olamaz.");

        var entity = await _repository.GetByIdAsync(dto.Id);
        if (entity == null)
            return (false, "Kategori bulunamadı.");

        var existing = await _repository.GetByNameAsync(dto.Name);
        if (existing != null && existing.Id != dto.Id)
            return (false, $"'{dto.Name}' adında başka bir kategori zaten mevcut.");

        entity.Name = dto.Name.Trim();
        entity.Description = dto.Description?.Trim();

        _repository.Update(entity);
        await _repository.SaveChangesAsync();
        return (true, "Kategori başarıyla güncellendi.");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return (false, "Kategori bulunamadı.");

        var hasProducts = await _repository.HasProductsAsync(id);
        if (hasProducts)
            return (false, "Bu kategoriye ait ürünler bulunduğu için silinemez.");

        _repository.Delete(entity);
        await _repository.SaveChangesAsync();
        return (true, "Kategori başarıyla silindi.");
    }
}
