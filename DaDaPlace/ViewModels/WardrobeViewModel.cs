using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DaDaPlace.Models;
using DaDaPlace.Services;
using Microsoft.Extensions.Logging;

namespace DaDaPlace.ViewModels;

/// <summary>
/// 衣橱页面ViewModel
/// </summary>
public partial class WardrobeViewModel : ViewModelBase
{
    private readonly IClothingService _clothingService;
    private readonly ILogger<WardrobeViewModel>? _logger;
    
    [ObservableProperty]
    private ObservableCollection<ClothingItem> _clothingItems = new();
    
    [ObservableProperty]
    private ObservableCollection<ClothingItem> _filteredClothingItems = new();
    
    [ObservableProperty]
    private string _searchText = string.Empty;
    
    [ObservableProperty]
    private ClothingCategory? _selectedCategory;
    
    [ObservableProperty]
    private Season? _selectedSeason;
    
    [ObservableProperty]
    private Occasion? _selectedOccasion;
    
    [ObservableProperty]
    private string _selectedColor = string.Empty;
    
    [ObservableProperty]
    private bool _isLoading = false;
    
    [ObservableProperty]
    private bool _isFilterPanelOpen = false;
    
    [ObservableProperty]
    private ClothingItem? _selectedItem;
    
    [ObservableProperty]
    private bool _isMultiSelectMode = false;
    
    [ObservableProperty]
    private ObservableCollection<ClothingItem> _selectedItems = new();
    
    public ObservableCollection<ClothingCategory> Categories { get; }
    public ObservableCollection<Season> Seasons { get; }
    public ObservableCollection<Occasion> Occasions { get; }
    public ObservableCollection<string> Colors { get; }
    
    public WardrobeViewModel(IClothingService? clothingService = null, ILogger<WardrobeViewModel>? logger = null)
    {
        _clothingService = clothingService ?? new ClothingService(new Data.AppDbContext(), Microsoft.Extensions.Logging.Abstractions.NullLogger<ClothingService>.Instance);
        _logger = logger;
        
        // 初始化筛选选项
        Categories = new ObservableCollection<ClothingCategory>(Enum.GetValues<ClothingCategory>());
        Seasons = new ObservableCollection<Season>(Enum.GetValues<Season>());
        Occasions = new ObservableCollection<Occasion>(Enum.GetValues<Occasion>());
        Colors = new ObservableCollection<string>
        {
            "白色", "黑色", "灰色", "红色", "蓝色", "绿色", "黄色", "紫色", "粉色", "棕色", "米色", "卡其色"
        };
        
        // 加载数据
        _ = LoadClothingItemsAsync();
    }
    
    partial void OnSearchTextChanged(string value)
    {
        _ = ApplyFiltersAsync();
    }
    
    partial void OnSelectedCategoryChanged(ClothingCategory? value)
    {
        _ = ApplyFiltersAsync();
    }
    
    partial void OnSelectedSeasonChanged(Season? value)
    {
        _ = ApplyFiltersAsync();
    }
    
    partial void OnSelectedOccasionChanged(Occasion? value)
    {
        _ = ApplyFiltersAsync();
    }
    
    partial void OnSelectedColorChanged(string value)
    {
        _ = ApplyFiltersAsync();
    }
    
    [RelayCommand]
    private async Task LoadClothingItemsAsync()
    {
        try
        {
            IsLoading = true;
            var items = await _clothingService.GetAllClothingItemsAsync();
            
            ClothingItems.Clear();
            foreach (var item in items)
            {
                ClothingItems.Add(item);
            }
            
            await ApplyFiltersAsync();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "加载衣橱数据失败");
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    private async Task ApplyFiltersAsync()
    {
        try
        {
            var filtered = ClothingItems.AsEnumerable();
            
            // 搜索文本筛选
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(x => 
                    x.Category2.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    x.Brand.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    x.Note.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    x.BaseColor.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }
            
            // 分类筛选
            if (SelectedCategory.HasValue)
            {
                filtered = filtered.Where(x => x.Category1 == SelectedCategory.Value);
            }
            
            // 季节筛选
            if (SelectedSeason.HasValue)
            {
                filtered = filtered.Where(x => x.SeasonTags.Contains(SelectedSeason.Value));
            }
            
            // 场合筛选
            if (SelectedOccasion.HasValue)
            {
                filtered = filtered.Where(x => x.Occasions.Contains(SelectedOccasion.Value));
            }
            
            // 颜色筛选
            if (!string.IsNullOrWhiteSpace(SelectedColor))
            {
                filtered = filtered.Where(x => x.BaseColor.Contains(SelectedColor, StringComparison.OrdinalIgnoreCase));
            }
            
            FilteredClothingItems.Clear();
            foreach (var item in filtered.OrderByDescending(x => x.CreatedAt))
            {
                FilteredClothingItems.Add(item);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "应用筛选条件失败");
        }
    }
    
    [RelayCommand]
    private void ToggleFilterPanel()
    {
        IsFilterPanelOpen = !IsFilterPanelOpen;
    }
    
    [RelayCommand]
    private void ClearFilters()
    {
        SearchText = string.Empty;
        SelectedCategory = null;
        SelectedSeason = null;
        SelectedOccasion = null;
        SelectedColor = string.Empty;
    }
    
    [RelayCommand]
    private async Task AddNewClothingAsync()
    {
        try
        {
            // TODO: 打开添加新服装的对话框或页面
            // 这里可以打开文件选择器选择图片，然后进行AI识别
            _logger?.LogInformation("准备添加新服装");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "添加新服装失败");
        }
    }
    
    [RelayCommand]
    private async Task ToggleFavoriteAsync(ClothingItem item)
    {
        try
        {
            await _clothingService.ToggleFavoriteAsync(item.ItemId);
            item.IsFavorite = !item.IsFavorite;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"切换收藏状态失败: {item.ItemId}");
        }
    }
    
    [RelayCommand]
    private void SelectItem(ClothingItem item)
    {
        if (IsMultiSelectMode)
        {
            if (SelectedItems.Contains(item))
            {
                SelectedItems.Remove(item);
            }
            else
            {
                SelectedItems.Add(item);
            }
        }
        else
        {
            SelectedItem = item;
            // TODO: 打开单品详情页面
        }
    }
    
    [RelayCommand]
    private void ToggleMultiSelectMode()
    {
        IsMultiSelectMode = !IsMultiSelectMode;
        if (!IsMultiSelectMode)
        {
            SelectedItems.Clear();
        }
    }
    
    [RelayCommand]
    private async Task DeleteSelectedItemsAsync()
    {
        try
        {
            if (!SelectedItems.Any())
                return;
                
            // TODO: 显示确认对话框
            
            foreach (var item in SelectedItems.ToList())
            {
                await _clothingService.DeleteClothingItemAsync(item.ItemId);
                ClothingItems.Remove(item);
                FilteredClothingItems.Remove(item);
            }
            
            SelectedItems.Clear();
            IsMultiSelectMode = false;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "删除选中的服装失败");
        }
    }
    
    [RelayCommand]
    private async Task EditItemAsync(ClothingItem item)
    {
        try
        {
            // TODO: 打开编辑服装的对话框或页面
            _logger?.LogInformation($"编辑服装: {item.ItemId}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"编辑服装失败: {item.ItemId}");
        }
    }
    
    [RelayCommand]
    private void ViewItemDetail(ClothingItem item)
    {
        SelectedItem = item;
        // TODO: 打开服装详情页面
    }
}