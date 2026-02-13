using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using MiniERP.BL.DTOs;
using MiniERP.BL.Services;
using MiniERP.UI.Helpers;

namespace MiniERP.UI.ViewModels;

public class CategoryViewModel : BaseViewModel, ILoadableViewModel
{
    private readonly ICategoryService _categoryService;

    public CategoryViewModel(ICategoryService categoryService)
    {
        _categoryService = categoryService;

        AddCommand = new AsyncRelayCommand(AddCategoryAsync);
        UpdateCommand = new AsyncRelayCommand(UpdateCategoryAsync);
        DeleteCommand = new AsyncRelayCommand(DeleteCategoryAsync);
        ClearCommand = new RelayCommand(ClearForm);
    }

    // Collections
    private ObservableCollection<CategoryDto> _categories = new();
    public ObservableCollection<CategoryDto> Categories
    {
        get => _categories;
        set => SetProperty(ref _categories, value);
    }

    // Form fields
    private int _selectedId;
    public int SelectedId { get => _selectedId; set => SetProperty(ref _selectedId, value); }

    private string _name = string.Empty;
    public string Name { get => _name; set => SetProperty(ref _name, value); }

    private string? _description;
    public string? Description { get => _description; set => SetProperty(ref _description, value); }

    private CategoryDto? _selectedCategory;
    public CategoryDto? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            SetProperty(ref _selectedCategory, value);
            if (value != null)
            {
                SelectedId = value.Id;
                Name = value.Name;
                Description = value.Description;
            }
        }
    }

    private bool _isEditing;
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }

    // Commands
    public ICommand AddCommand { get; }
    public ICommand UpdateCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ClearCommand { get; }

    public async Task LoadDataAsync()
    {
        IsBusy = true;
        try
        {
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

    private async Task AddCategoryAsync()
    {
        var dto = new CategoryDto { Name = Name, Description = Description };
        var (success, message) = await _categoryService.AddAsync(dto);
        StatusMessage = message;
        if (success)
        {
            ClearForm();
            await LoadDataAsync();
        }
    }

    private async Task UpdateCategoryAsync()
    {
        if (SelectedId == 0) { StatusMessage = "Lütfen güncellenecek kategoriyi seçin."; return; }
        var dto = new CategoryDto { Id = SelectedId, Name = Name, Description = Description };
        var (success, message) = await _categoryService.UpdateAsync(dto);
        StatusMessage = message;
        if (success)
        {
            ClearForm();
            await LoadDataAsync();
        }
    }

    private async Task DeleteCategoryAsync()
    {
        if (SelectedId == 0) { StatusMessage = "Lütfen silinecek kategoriyi seçin."; return; }
        var result = MessageBox.Show("Bu kategoriyi silmek istediğinize emin misiniz?", "Onay",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result != MessageBoxResult.Yes) return;

        var (success, message) = await _categoryService.DeleteAsync(SelectedId);
        StatusMessage = message;
        if (success)
        {
            ClearForm();
            await LoadDataAsync();
        }
    }

    private void ClearForm()
    {
        SelectedId = 0;
        Name = string.Empty;
        Description = null;
        SelectedCategory = null;
        IsEditing = false;
    }
}
