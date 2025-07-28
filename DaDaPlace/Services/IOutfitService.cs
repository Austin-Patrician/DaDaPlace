using System.Collections.Generic;
using System.Threading.Tasks;
using DaDaPlace.Models;

namespace DaDaPlace.Services;

/// <summary>
/// AI搭配服务接口
/// </summary>
public interface IOutfitService
{
    /// <summary>
    /// AI一键搭配
    /// </summary>
    Task<List<OutfitLook>> GenerateOutfitRecommendationsAsync(OutfitRequest request);
    
    /// <summary>
    /// 基于起始单品的场景化搭配
    /// </summary>
    Task<List<OutfitLook>> GenerateOutfitFromStartingItemAsync(int startingItemId, OutfitRequest request);
    
    /// <summary>
    /// 保存搭配方案
    /// </summary>
    Task<OutfitLook> SaveOutfitLookAsync(OutfitLook outfitLook);
    
    /// <summary>
    /// 获取所有搭配方案
    /// </summary>
    Task<List<OutfitLook>> GetAllOutfitLooksAsync();
    
    /// <summary>
    /// 根据ID获取搭配方案
    /// </summary>
    Task<OutfitLook?> GetOutfitLookByIdAsync(int lookId);
    
    /// <summary>
    /// 删除搭配方案
    /// </summary>
    Task<bool> DeleteOutfitLookAsync(int lookId);
    
    /// <summary>
    /// 分析衣橱缺口
    /// </summary>
    Task<List<ClothingGap>> AnalyzeWardrobeGapsAsync(OutfitRequest request);
    
    /// <summary>
    /// 生成搭配推荐理由
    /// </summary>
    Task<string> GenerateRecommendationReasonAsync(List<ClothingItem> items, OutfitRequest request);
    
    /// <summary>
    /// 计算搭配评分
    /// </summary>
    Task<OutfitScore> CalculateOutfitScoreAsync(List<ClothingItem> items, OutfitRequest request);
    
    /// <summary>
    /// 生成虚拟模特搭配图
    /// </summary>
    Task<string> GenerateVirtualModelImageAsync(List<ClothingItem> items, UserProfile userProfile);
}

/// <summary>
/// 衣橱缺口分析结果
/// </summary>
public class ClothingGap
{
    /// <summary>
    /// 缺少的服装类型
    /// </summary>
    public ClothingCategory Category { get; set; }
    
    /// <summary>
    /// 具体描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// 推荐购买的单品
    /// </summary>
    public List<string> RecommendedItems { get; set; } = new();
    
    /// <summary>
    /// 优先级(1-10)
    /// </summary>
    public int Priority { get; set; }
}

/// <summary>
/// 搭配评分
/// </summary>
public class OutfitScore
{
    /// <summary>
    /// 天气匹配度评分(1-10)
    /// </summary>
    public int WeatherMatchScore { get; set; }
    
    /// <summary>
    /// 色系协调度评分(1-10)
    /// </summary>
    public int ColorHarmonyScore { get; set; }
    
    /// <summary>
    /// 风格一致性评分(1-10)
    /// </summary>
    public int StyleConsistencyScore { get; set; }
    
    /// <summary>
    /// 总体评分(1-10)
    /// </summary>
    public int OverallScore => (WeatherMatchScore + ColorHarmonyScore + StyleConsistencyScore) / 3;
}