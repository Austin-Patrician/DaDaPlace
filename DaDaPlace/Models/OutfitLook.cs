using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DaDaPlace.Models;

/// <summary>
/// 搭配方案数据模型
/// </summary>
public class OutfitLook
{
    [Key]
    public int LookId { get; set; }
    
    /// <summary>
    /// 搭配名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 搭配描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// 搭配图片URI
    /// </summary>
    public string ImageUri { get; set; } = string.Empty;
    
    /// <summary>
    /// 包含的服装单品ID列表
    /// </summary>
    public List<int> ClothingItemIds { get; set; } = new();
    
    /// <summary>
    /// 适合的天气温度
    /// </summary>
    public int? Temperature { get; set; }
    
    /// <summary>
    /// 适合的场合
    /// </summary>
    public List<Occasion> Occasions { get; set; } = new();
    
    /// <summary>
    /// AI推荐理由
    /// </summary>
    public string RecommendationReason { get; set; } = string.Empty;
    
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
    /// 是否公开
    /// </summary>
    public bool IsPublic { get; set; }
    
    /// <summary>
    /// 创建者用户ID
    /// </summary>
    public int CreatorUserId { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

/// <summary>
/// 搭配请求参数
/// </summary>
public class OutfitRequest
{
    /// <summary>
    /// 今日温度
    /// </summary>
    public int Temperature { get; set; }
    
    /// <summary>
    /// 场合
    /// </summary>
    public Occasion Occasion { get; set; }
    
    /// <summary>
    /// 起始单品ID（自选场景）
    /// </summary>
    public int? StartingItemId { get; set; }
    
    /// <summary>
    /// 用户偏好风格
    /// </summary>
    public StylePreference? PreferredStyle { get; set; }
}