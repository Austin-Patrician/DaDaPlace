using System;
using System.Collections.Generic;
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
/// 搭配页面ViewModel
/// </summary>
public partial class OutfitViewModel : ViewModelBase
{
    private readonly IOutfitService _outfitService;
    private readonly IClothingService _clothingService;
    private readonly ILogger<OutfitViewModel>? _logger;
    
    [ObservableProperty]
    private ObservableCollection<OutfitLook> _savedOutfits = new();
    
    [ObservableProperty]
    private ObservableCollection<OutfitLook> _recommendedOutfits = new();
    
    [ObservableProperty]
    private OutfitLook? _currentOutfit;
    
    [ObservableProperty]
    private bool _isGenerating = false;
    
    [ObservableProperty]
    private bool _isLoading = false;
    
    [ObservableProperty]
    private string _selectedWeather = "晴天";
    
    [ObservableProperty]
    private double _temperature = 20.0;
    
    [ObservableProperty]
    private Occasion _selectedOccasion = Occasion.休闲;
    
    [ObservableProperty]
    private ClothingItem? _startingItem;
    
    [ObservableProperty]
    private string _outfitMode = "AI一键搭配"; // "AI一键搭配" 或 "自选场景"
    
    [ObservableProperty]
    private ObservableCollection<ClothingItem> _availableItems = new();
    
    [ObservableProperty]
    private ObservableCollection<ClothingGap> _wardrobeGaps = new();
    
    [ObservableProperty]
    private string _recommendationReason = string.Empty;
    
    [ObservableProperty]
    private OutfitScore? _currentOutfitScore;
    
    public ObservableCollection<string> WeatherOptions { get; }
    public ObservableCollection<Occasion> OccasionOptions { get; }
    public ObservableCollection<string> OutfitModes { get; }
    
    public OutfitViewModel(IOutfitService? outfitService = null, IClothingService? clothingService = null, ILogger<OutfitViewModel>? logger = null)
    {
        _outfitService = outfitService ?? new OutfitService(new Data.AppDbContext(), new ClothingService(new Data.AppDbContext(), Microsoft.Extensions.Logging.Abstractions.NullLogger<ClothingService>.Instance), Microsoft.Extensions.Logging.Abstractions.NullLogger<OutfitService>.Instance);
        _clothingService = clothingService ?? new ClothingService(new Data.AppDbContext(), Microsoft.Extensions.Logging.Abstractions.NullLogger<ClothingService>.Instance);
        _logger = logger;
        
        // 初始化选项
        WeatherOptions = new ObservableCollection<string>
        {
            "晴天", "阴天", "雨天", "雪天", "多云", "大风"
        };
        
        OccasionOptions = new ObservableCollection<Occasion>(Enum.GetValues<Occasion>());
        
        OutfitModes = new ObservableCollection<string>
        {
            "AI一键搭配", "自选场景"
        };
        
        // 加载数据
        _ = LoadDataAsync();
    }
    
    partial void OnOutfitModeChanged(string value)
    {
        if (value == "自选场景")
        {
            _ = LoadAvailableItemsAsync();
        }
    }
    
    [RelayCommand]
    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            
            // 加载已保存的搭配
            var savedOutfits = await _outfitService.GetAllOutfitLooksAsync();
            SavedOutfits.Clear();
            foreach (var outfit in savedOutfits)
            {
                SavedOutfits.Add(outfit);
            }
            
            // 分析衣橱缺口
            await AnalyzeWardrobeGapsAsync();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "加载搭配数据失败");
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    private async Task LoadAvailableItemsAsync()
    {
        try
        {
            var items = await _clothingService.GetAllClothingItemsAsync();
            AvailableItems.Clear();
            foreach (var item in items)
            {
                AvailableItems.Add(item);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "加载可用服装失败");
        }
    }
    
    [RelayCommand]
    private async Task GenerateAIOutfitAsync()
    {
        try
        {
            IsGenerating = true;
            RecommendedOutfits.Clear();
            
            var request = new OutfitRequest
            {
                Temperature = (int)Math.Round(Temperature),
                Occasion = SelectedOccasion,
                StartingItemId = StartingItem?.ItemId
            };
            
            List<OutfitLook> recommendations;
            
            if (OutfitMode == "AI一键搭配")
            {
                recommendations = await _outfitService.GenerateOutfitRecommendationsAsync(request);
            }
            else
            {
                if (StartingItem == null)
                {
                    _logger?.LogWarning("自选场景模式需要选择起始单品");
                    return;
                }
                recommendations = await _outfitService.GenerateOutfitFromStartingItemAsync(StartingItem.ItemId, request);
            }
            
            foreach (var outfit in recommendations)
            {
                RecommendedOutfits.Add(outfit);
            }
            
            // 如果有推荐结果，选择第一个
            if (RecommendedOutfits.Any())
            {
                await SelectOutfitAsync(RecommendedOutfits.First());
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "生成AI搭配推荐失败");
        }
        finally
        {
            IsGenerating = false;
        }
    }
    
    [RelayCommand]
    private async Task SelectOutfitAsync(OutfitLook outfit)
    {
        try
        {
            CurrentOutfit = outfit;
            
            // 通过ClothingItemIds获取对应的ClothingItem列表
            var items = new List<ClothingItem>();
            foreach (var itemId in outfit.ClothingItemIds)
            {
                var item = await _clothingService.GetClothingItemByIdAsync(itemId);
                if (item != null)
                {
                    items.Add(item);
                }
            }
            
            var request = new OutfitRequest
            {
                Temperature = outfit.Temperature ?? 0,
                Occasion = SelectedOccasion
            };
            
            // 生成推荐理由
            RecommendationReason = await _outfitService.GenerateRecommendationReasonAsync(items, request);
            
            // 计算搭配评分
            CurrentOutfitScore = await _outfitService.CalculateOutfitScoreAsync(items, request);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "选择搭配失败");
        }
    }
    
    [RelayCommand]
    private async Task SaveCurrentOutfitAsync()
    {
        try
        {
            if (CurrentOutfit == null)
                return;
                
            var savedOutfit = await _outfitService.SaveOutfitLookAsync(CurrentOutfit);
            
            // 添加到已保存列表
            if (!SavedOutfits.Any(x => x.LookId == savedOutfit.LookId))
            {
                SavedOutfits.Insert(0, savedOutfit);
            }
            
            _logger?.LogInformation($"搭配已保存: {savedOutfit.Name}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "保存搭配失败");
        }
    }
    
    [RelayCommand]
    private async Task DeleteSavedOutfitAsync(OutfitLook outfit)
    {
        try
        {
            await _outfitService.DeleteOutfitLookAsync(outfit.LookId);
            SavedOutfits.Remove(outfit);
            
            if (CurrentOutfit?.LookId == outfit.LookId)
            {
                CurrentOutfit = null;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"删除搭配失败: {outfit.LookId}");
        }
    }
    
    [RelayCommand]
    private void SelectStartingItem(ClothingItem item)
    {
        StartingItem = item;
    }
    
    [RelayCommand]
    private async Task AnalyzeWardrobeGapsAsync()
    {
        try
        {
            var request = new OutfitRequest
            {
                Temperature = (int)Math.Round(Temperature),
                Occasion = SelectedOccasion
            };
            
            var gaps = await _outfitService.AnalyzeWardrobeGapsAsync(request);
            WardrobeGaps.Clear();
            foreach (var gap in gaps)
            {
                WardrobeGaps.Add(gap);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "分析衣橱缺口失败");
        }
    }
    
    [RelayCommand]
    private async Task ShareOutfitAsync(OutfitLook outfit)
    {
        try
        {
            // TODO: 实现分享功能
            // 生成分享图片，支持分享到社交平台
            _logger?.LogInformation($"分享搭配: {outfit.Name}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"分享搭配失败: {outfit.LookId}");
        }
    }
    
    [RelayCommand]
    private async Task GenerateVirtualModelImageAsync(OutfitLook outfit)
    {
        try
        {
            // TODO: 调用虚拟模特生成服务
            var userProfile = new UserProfile { UserId = 1 }; // TODO: 获取当前用户资料
            
            // 通过ClothingItemIds获取对应的ClothingItem列表
            var items = new List<ClothingItem>();
            foreach (var itemId in outfit.ClothingItemIds)
            {
                var item = await _clothingService.GetClothingItemByIdAsync(itemId);
                if (item != null)
                {
                    items.Add(item);
                }
            }
            
            var virtualImageUri = await _outfitService.GenerateVirtualModelImageAsync(items, userProfile);
            outfit.ImageUri = virtualImageUri;
            
            _logger?.LogInformation($"虚拟模特图片生成完成: {outfit.Name}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"生成虚拟模特图片失败: {outfit.LookId}");
        }
    }
    
    [RelayCommand]
    private void RefreshRecommendations()
    {
        _ = GenerateAIOutfitAsync();
    }
    
    [RelayCommand]
    private void ClearCurrentOutfit()
    {
        CurrentOutfit = null;
        RecommendationReason = string.Empty;
        CurrentOutfitScore = null;
    }
    
    [RelayCommand]
    private async Task LoadSavedOutfitAsync(OutfitLook outfit)
    {
        try
        {
            await SelectOutfitAsync(outfit);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"加载已保存搭配失败: {outfit.LookId}");
        }
    }
}