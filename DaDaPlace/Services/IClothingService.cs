using System.Collections.Generic;
using System.Threading.Tasks;
using DaDaPlace.Models;

namespace DaDaPlace.Services;

/// <summary>
/// 衣橱管理服务接口
/// </summary>
public interface IClothingService
{
    /// <summary>
    /// 添加服装单品
    /// </summary>
    Task<ClothingItem> AddClothingItemAsync(ClothingItem item);
    
    /// <summary>
    /// 获取所有服装单品
    /// </summary>
    Task<List<ClothingItem>> GetAllClothingItemsAsync();
    
    /// <summary>
    /// 根据ID获取服装单品
    /// </summary>
    Task<ClothingItem?> GetClothingItemByIdAsync(int itemId);
    
    /// <summary>
    /// 更新服装单品
    /// </summary>
    Task<ClothingItem> UpdateClothingItemAsync(ClothingItem item);
    
    /// <summary>
    /// 删除服装单品
    /// </summary>
    Task<bool> DeleteClothingItemAsync(int itemId);
    
    /// <summary>
    /// 根据分类筛选服装
    /// </summary>
    Task<List<ClothingItem>> GetClothingItemsByCategoryAsync(ClothingCategory category);
    
    /// <summary>
    /// 根据季节筛选服装
    /// </summary>
    Task<List<ClothingItem>> GetClothingItemsBySeasonAsync(Season season);
    
    /// <summary>
    /// 根据场合筛选服装
    /// </summary>
    Task<List<ClothingItem>> GetClothingItemsByOccasionAsync(Occasion occasion);
    
    /// <summary>
    /// 根据颜色筛选服装
    /// </summary>
    Task<List<ClothingItem>> GetClothingItemsByColorAsync(string color);
    
    /// <summary>
    /// 搜索服装单品
    /// </summary>
    Task<List<ClothingItem>> SearchClothingItemsAsync(string keyword);
    
    /// <summary>
    /// 获取收藏的服装单品
    /// </summary>
    Task<List<ClothingItem>> GetFavoriteClothingItemsAsync();
    
    /// <summary>
    /// 切换收藏状态
    /// </summary>
    Task<bool> ToggleFavoriteAsync(int itemId);
    
    /// <summary>
    /// 增加穿着次数
    /// </summary>
    Task<bool> IncrementWornCountAsync(int itemId);
    
    /// <summary>
    /// AI识别服装分类和颜色
    /// </summary>
    Task<(ClothingCategory category, string color)> AnalyzeClothingImageAsync(string imagePath);
    
    /// <summary>
    /// 自动裁剪服装图片
    /// </summary>
    Task<string> CropClothingImageAsync(string imagePath);
}