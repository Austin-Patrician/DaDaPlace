using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DaDaPlace.Data;
using DaDaPlace.Models;
using Microsoft.Extensions.Logging;

namespace DaDaPlace.Services;

/// <summary>
/// AI搭配服务实现
/// </summary>
public class OutfitService : IOutfitService
{
    private readonly AppDbContext _context;
    private readonly IClothingService _clothingService;
    private readonly ILogger<OutfitService> _logger;
    
    public OutfitService(AppDbContext context, IClothingService clothingService, ILogger<OutfitService> logger)
    {
        _context = context;
        _clothingService = clothingService;
        _logger = logger;
    }
    
    public async Task<List<OutfitLook>> GenerateOutfitRecommendationsAsync(OutfitRequest request)
    {
        try
        {
            _logger.LogInformation($"开始生成AI搭配推荐: 温度{request.Temperature}°C, 场合{request.Occasion}");
            
            // 获取所有可用的服装单品
            var allItems = await _clothingService.GetAllClothingItemsAsync();
            
            // 根据温度和季节筛选合适的服装
            var suitableItems = FilterItemsByTemperature(allItems, request.Temperature);
            
            // 根据场合筛选服装
            suitableItems = FilterItemsByOccasion(suitableItems, request.Occasion);
            
            // 生成搭配组合
            var outfitCombinations = await GenerateOutfitCombinationsAsync(suitableItems, request);
            
            // 评分和排序
            var rankedOutfits = new List<OutfitLook>();
            foreach (var combination in outfitCombinations.Take(5)) // 取前5个组合
            {
                var score = await CalculateOutfitScoreAsync(combination, request);
                var reason = await GenerateRecommendationReasonAsync(combination, request);
                
                var outfit = new OutfitLook
                {
                    Name = $"{request.Occasion}搭配方案",
                    Description = "AI智能推荐搭配",
                    ClothingItemIds = combination.Select(x => x.ItemId).ToList(),
                    Temperature = request.Temperature,
                    Occasions = new List<Occasion> { request.Occasion },
                    RecommendationReason = reason,
                    WeatherMatchScore = score.WeatherMatchScore,
                    ColorHarmonyScore = score.ColorHarmonyScore,
                    StyleConsistencyScore = score.StyleConsistencyScore,
                    CreatorUserId = 0, // AI生成
                    IsPublic = false
                };
                
                rankedOutfits.Add(outfit);
            }
            
            _logger.LogInformation($"生成了{rankedOutfits.Count}个搭配推荐");
            return rankedOutfits.OrderByDescending(x => x.ColorHarmonyScore + x.WeatherMatchScore + x.StyleConsistencyScore).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成AI搭配推荐失败");
            throw;
        }
    }
    
    public async Task<List<OutfitLook>> GenerateOutfitFromStartingItemAsync(int startingItemId, OutfitRequest request)
    {
        try
        {
            _logger.LogInformation($"基于起始单品{startingItemId}生成搭配");
            
            var startingItem = await _clothingService.GetClothingItemByIdAsync(startingItemId);
            if (startingItem == null)
            {
                throw new ArgumentException($"找不到起始单品: {startingItemId}");
            }
            
            // 获取所有其他服装单品
            var allItems = await _clothingService.GetAllClothingItemsAsync();
            var otherItems = allItems.Where(x => x.ItemId != startingItemId).ToList();
            
            // 根据起始单品的特征筛选兼容的服装
            var compatibleItems = FilterCompatibleItems(otherItems, startingItem, request);
            
            // 生成包含起始单品的搭配组合
            var outfitCombinations = await GenerateOutfitCombinationsWithStartingItemAsync(startingItem, compatibleItems, request);
            
            // 评分和排序
            var rankedOutfits = new List<OutfitLook>();
            foreach (var combination in outfitCombinations.Take(3)) // 取前3个组合
            {
                var score = await CalculateOutfitScoreAsync(combination, request);
                var reason = await GenerateRecommendationReasonAsync(combination, request);
                
                var outfit = new OutfitLook
                {
                    Name = $"基于{startingItem.Category2}的搭配",
                    Description = "基于选定单品的智能搭配",
                    ClothingItemIds = combination.Select(x => x.ItemId).ToList(),
                    Temperature = request.Temperature,
                    Occasions = new List<Occasion> { request.Occasion },
                    RecommendationReason = reason,
                    WeatherMatchScore = score.WeatherMatchScore,
                    ColorHarmonyScore = score.ColorHarmonyScore,
                    StyleConsistencyScore = score.StyleConsistencyScore,
                    CreatorUserId = 0, // AI生成
                    IsPublic = false
                };
                
                rankedOutfits.Add(outfit);
            }
            
            _logger.LogInformation($"基于起始单品生成了{rankedOutfits.Count}个搭配推荐");
            return rankedOutfits.OrderByDescending(x => x.ColorHarmonyScore + x.WeatherMatchScore + x.StyleConsistencyScore).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"基于起始单品生成搭配失败: {startingItemId}");
            throw;
        }
    }
    
    public async Task<OutfitLook> SaveOutfitLookAsync(OutfitLook outfitLook)
    {
        try
        {
            outfitLook.CreatedAt = DateTime.Now;
            outfitLook.UpdatedAt = DateTime.Now;
            
            _context.OutfitLooks.Add(outfitLook);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"保存搭配方案成功: {outfitLook.LookId}");
            return outfitLook;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存搭配方案失败");
            throw;
        }
    }
    
    public async Task<List<OutfitLook>> GetAllOutfitLooksAsync()
    {
        try
        {
            return await _context.OutfitLooks
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取所有搭配方案失败");
            throw;
        }
    }
    
    public async Task<OutfitLook?> GetOutfitLookByIdAsync(int lookId)
    {
        try
        {
            return await _context.OutfitLooks
                .FirstOrDefaultAsync(x => x.LookId == lookId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"获取搭配方案失败: {lookId}");
            throw;
        }
    }
    
    public async Task<bool> DeleteOutfitLookAsync(int lookId)
    {
        try
        {
            var outfit = await _context.OutfitLooks.FindAsync(lookId);
            if (outfit == null)
                return false;
                
            _context.OutfitLooks.Remove(outfit);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"删除搭配方案成功: {lookId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"删除搭配方案失败: {lookId}");
            throw;
        }
    }
    
    public async Task<List<ClothingGap>> AnalyzeWardrobeGapsAsync(OutfitRequest request)
    {
        try
        {
            _logger.LogInformation("开始分析衣橱缺口");
            
            var allItems = await _clothingService.GetAllClothingItemsAsync();
            var gaps = new List<ClothingGap>();
            
            // 分析各类别的服装数量
            var categoryGroups = allItems.GroupBy(x => x.Category1).ToList();
            
            // 检查基础单品是否齐全
            if (!categoryGroups.Any(g => g.Key == ClothingCategory.上衣))
            {
                gaps.Add(new ClothingGap
                {
                    Category = ClothingCategory.上衣,
                    Description = "缺少基础上衣",
                    RecommendedItems = new List<string> { "白色T恤", "基础衬衫", "针织衫" },
                    Priority = 10
                });
            }
            
            if (!categoryGroups.Any(g => g.Key == ClothingCategory.下装))
            {
                gaps.Add(new ClothingGap
                {
                    Category = ClothingCategory.下装,
                    Description = "缺少基础下装",
                    RecommendedItems = new List<string> { "牛仔裤", "休闲裤", "半身裙" },
                    Priority = 9
                });
            }
            
            // 根据场合分析缺口
            var occasionItems = allItems.Where(x => x.Occasions.Contains(request.Occasion)).ToList();
            if (occasionItems.Count < 3)
            {
                gaps.Add(new ClothingGap
                {
                    Category = ClothingCategory.上衣,
                    Description = $"缺少适合{request.Occasion}场合的服装",
                    RecommendedItems = GetRecommendedItemsForOccasion(request.Occasion),
                    Priority = 8
                });
            }
            
            _logger.LogInformation($"分析出{gaps.Count}个衣橱缺口");
            return gaps.OrderByDescending(x => x.Priority).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "分析衣橱缺口失败");
            throw;
        }
    }
    
    public async Task<string> GenerateRecommendationReasonAsync(List<ClothingItem> items, OutfitRequest request)
    {
        await Task.Delay(50); // 模拟AI处理时间
        
        var reasons = new List<string>();
        
        // 天气匹配分析
        if (request.Temperature < 10)
        {
            reasons.Add("适合低温天气，保暖性好");
        }
        else if (request.Temperature > 25)
        {
            reasons.Add("适合高温天气，透气清爽");
        }
        else
        {
            reasons.Add("适合当前温度，舒适度佳");
        }
        
        // 场合匹配分析
        reasons.Add($"符合{request.Occasion}场合的着装要求");
        
        // 色彩搭配分析
        var colors = items.Select(x => x.BaseColor).Distinct().ToList();
        if (colors.Count <= 3)
        {
            reasons.Add("色彩搭配和谐，层次分明");
        }
        
        // 风格一致性分析
        reasons.Add("整体风格统一，搭配协调");
        
        return string.Join("；", reasons);
    }
    
    public async Task<OutfitScore> CalculateOutfitScoreAsync(List<ClothingItem> items, OutfitRequest request)
    {
        await Task.Delay(50); // 模拟AI计算时间
        
        var score = new OutfitScore();
        
        // 天气匹配度评分
        score.WeatherMatchScore = CalculateWeatherMatchScore(items, request.Temperature);
        
        // 色系协调度评分
        score.ColorHarmonyScore = CalculateColorHarmonyScore(items);
        
        // 风格一致性评分
        score.StyleConsistencyScore = CalculateStyleConsistencyScore(items, request.Occasion);
        
        return score;
    }
    
    public async Task<string> GenerateVirtualModelImageAsync(List<ClothingItem> items, UserProfile userProfile)
    {
        // TODO: 实现虚拟模特图像生成
        // 这里可以集成TensorFlow Lite pose或其他3D渲染技术
        await Task.Delay(1000); // 模拟图像生成时间
        
        _logger.LogInformation($"为用户{userProfile.UserId}生成虚拟模特搭配图");
        
        // 临时返回占位符路径
        return "/Assets/virtual_model_placeholder.png";
    }
    
    #region 私有辅助方法
    
    private List<ClothingItem> FilterItemsByTemperature(List<ClothingItem> items, int temperature)
    {
        if (temperature < 10)
        {
            return items.Where(x => x.SeasonTags.Contains(Season.冬季) || x.SeasonTags.Contains(Season.秋季)).ToList();
        }
        else if (temperature > 25)
        {
            return items.Where(x => x.SeasonTags.Contains(Season.夏季) || x.SeasonTags.Contains(Season.春季)).ToList();
        }
        else
        {
            return items.Where(x => x.SeasonTags.Contains(Season.春季) || x.SeasonTags.Contains(Season.秋季)).ToList();
        }
    }
    
    private List<ClothingItem> FilterItemsByOccasion(List<ClothingItem> items, Occasion occasion)
    {
        return items.Where(x => x.Occasions.Contains(occasion) || x.Occasions.Contains(Occasion.休闲)).ToList();
    }
    
    private List<ClothingItem> FilterCompatibleItems(List<ClothingItem> items, ClothingItem startingItem, OutfitRequest request)
    {
        // 根据起始单品的特征筛选兼容的服装
        return items.Where(x => 
            x.Category1 != startingItem.Category1 && // 不同类别
            (x.Occasions.Any(o => startingItem.Occasions.Contains(o)) || x.Occasions.Contains(Occasion.休闲)) && // 场合兼容
            IsColorCompatible(x.BaseColor, startingItem.BaseColor) // 颜色兼容
        ).ToList();
    }
    
    private async Task<List<List<ClothingItem>>> GenerateOutfitCombinationsAsync(List<ClothingItem> items, OutfitRequest request)
    {
        await Task.Delay(100); // 模拟AI计算时间
        
        var combinations = new List<List<ClothingItem>>();
        
        // 按类别分组
        var tops = items.Where(x => x.Category1 == ClothingCategory.上衣).ToList();
        var bottoms = items.Where(x => x.Category1 == ClothingCategory.下装).ToList();
        var shoes = items.Where(x => x.Category1 == ClothingCategory.鞋子).ToList();
        var accessories = items.Where(x => x.Category1 == ClothingCategory.配饰).ToList();
        
        // 生成基础搭配组合（上衣+下装+鞋子）
        foreach (var top in tops.Take(3))
        {
            foreach (var bottom in bottoms.Take(3))
            {
                foreach (var shoe in shoes.Take(2))
                {
                    var combination = new List<ClothingItem> { top, bottom, shoe };
                    
                    // 可选添加配饰
                    if (accessories.Any())
                    {
                        combination.Add(accessories.First());
                    }
                    
                    combinations.Add(combination);
                }
            }
        }
        
        return combinations.Take(10).ToList(); // 限制组合数量
    }
    
    private async Task<List<List<ClothingItem>>> GenerateOutfitCombinationsWithStartingItemAsync(ClothingItem startingItem, List<ClothingItem> compatibleItems, OutfitRequest request)
    {
        await Task.Delay(100); // 模拟AI计算时间
        
        var combinations = new List<List<ClothingItem>>();
        
        // 根据起始单品类别生成不同的搭配策略
        switch (startingItem.Category1)
        {
            case ClothingCategory.上衣:
                var bottoms = compatibleItems.Where(x => x.Category1 == ClothingCategory.下装).Take(3);
                var shoes = compatibleItems.Where(x => x.Category1 == ClothingCategory.鞋子).Take(2);
                
                foreach (var bottom in bottoms)
                {
                    foreach (var shoe in shoes)
                    {
                        combinations.Add(new List<ClothingItem> { startingItem, bottom, shoe });
                    }
                }
                break;
                
            case ClothingCategory.下装:
                var tops = compatibleItems.Where(x => x.Category1 == ClothingCategory.上衣).Take(3);
                var shoesForBottoms = compatibleItems.Where(x => x.Category1 == ClothingCategory.鞋子).Take(2);
                
                foreach (var top in tops)
                {
                    foreach (var shoe in shoesForBottoms)
                    {
                        combinations.Add(new List<ClothingItem> { top, startingItem, shoe });
                    }
                }
                break;
                
            default:
                // 对于鞋子和配饰，生成包含上衣和下装的完整搭配
                var allTops = compatibleItems.Where(x => x.Category1 == ClothingCategory.上衣).Take(2);
                var allBottoms = compatibleItems.Where(x => x.Category1 == ClothingCategory.下装).Take(2);
                
                foreach (var top in allTops)
                {
                    foreach (var bottom in allBottoms)
                    {
                        combinations.Add(new List<ClothingItem> { top, bottom, startingItem });
                    }
                }
                break;
        }
        
        return combinations.Take(5).ToList();
    }
    
    private bool IsColorCompatible(string color1, string color2)
    {
        // 简单的颜色兼容性判断
        var neutralColors = new[] { "白色", "黑色", "灰色", "米色", "卡其色" };
        
        // 中性色与任何颜色都兼容
        if (neutralColors.Contains(color1) || neutralColors.Contains(color2))
            return true;
            
        // 同色系兼容
        if (color1 == color2)
            return true;
            
        // TODO: 实现更复杂的色彩理论判断
        return true; // 临时返回true
    }
    
    private int CalculateWeatherMatchScore(List<ClothingItem> items, int temperature)
    {
        // 根据温度和服装的季节标签计算匹配度
        int score = 5; // 基础分
        
        foreach (var item in items)
        {
            if (temperature < 10 && (item.SeasonTags.Contains(Season.冬季) || item.Volume > 3))
                score += 2;
            else if (temperature > 25 && (item.SeasonTags.Contains(Season.夏季) || item.Volume < 2))
                score += 2;
            else if (temperature >= 10 && temperature <= 25 && (item.SeasonTags.Contains(Season.春季) || item.SeasonTags.Contains(Season.秋季)))
                score += 1;
        }
        
        return Math.Min(score, 10);
    }
    
    private int CalculateColorHarmonyScore(List<ClothingItem> items)
    {
        // 简单的色彩和谐度计算
        var colors = items.Select(x => x.BaseColor).Distinct().ToList();
        
        if (colors.Count <= 3)
            return 8; // 颜色数量适中
        else if (colors.Count <= 4)
            return 6; // 颜色稍多
        else
            return 4; // 颜色过多
    }
    
    private int CalculateStyleConsistencyScore(List<ClothingItem> items, Occasion occasion)
    {
        // 计算风格一致性
        int score = 5;
        
        var occasionMatchCount = items.Count(x => x.Occasions.Contains(occasion));
        score += occasionMatchCount * 2;
        
        return Math.Min(score, 10);
    }
    
    private List<string> GetRecommendedItemsForOccasion(Occasion occasion)
    {
        return occasion switch
        {
            Occasion.工作 => new List<string> { "衬衫", "西装外套", "正装裤" },
            Occasion.运动 => new List<string> { "运动T恤", "运动裤", "运动鞋" },
            Occasion.正式 => new List<string> { "正装衬衫", "西装", "皮鞋" },
            Occasion.约会 => new List<string> { "连衣裙", "高跟鞋", "小包" },
            _ => new List<string> { "T恤", "牛仔裤", "休闲鞋" }
        };
    }
    
    #endregion
}