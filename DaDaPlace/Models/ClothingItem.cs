using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DaDaPlace.Models;

/// <summary>
/// 服装单品数据模型
/// </summary>
public class ClothingItem
{
    [Key]
    public int ItemId { get; set; }
    
    /// <summary>
    /// 原始图片URI
    /// </summary>
    public string Uri { get; set; } = string.Empty;
    
    /// <summary>
    /// 裁剪后的图片URI
    /// </summary>
    public string CropUri { get; set; } = string.Empty;
    
    /// <summary>
    /// 一级分类：上衣、下装、鞋子、配饰
    /// </summary>
    public ClothingCategory Category1 { get; set; }
    
    /// <summary>
    /// 二级分类：具体类型
    /// </summary>
    public string Category2 { get; set; } = string.Empty;
    
    /// <summary>
    /// 基础颜色
    /// </summary>
    public string BaseColor { get; set; } = string.Empty;
    
    /// <summary>
    /// 季节标签
    /// </summary>
    public List<Season> SeasonTags { get; set; } = new();
    
    /// <summary>
    /// 适合场合
    /// </summary>
    public List<Occasion> Occasions { get; set; } = new();
    
    /// <summary>
    /// 体积/厚度
    /// </summary>
    public int Volume { get; set; }
    
    /// <summary>
    /// 购买价格
    /// </summary>
    public decimal? BuyPrice { get; set; }
    
    /// <summary>
    /// 穿着次数
    /// </summary>
    public int WornCount { get; set; }
    
    /// <summary>
    /// 备注
    /// </summary>
    public string Note { get; set; } = string.Empty;
    
    /// <summary>
    /// 品牌
    /// </summary>
    public string Brand { get; set; } = string.Empty;
    
    /// <summary>
    /// 是否收藏
    /// </summary>
    public bool IsFavorite { get; set; }
    
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
/// 服装一级分类
/// </summary>
public enum ClothingCategory
{
    上衣 = 1,
    下装 = 2,
    鞋子 = 3,
    配饰 = 4
}

/// <summary>
/// 季节
/// </summary>
public enum Season
{
    春季 = 1,
    夏季 = 2,
    秋季 = 3,
    冬季 = 4
}

/// <summary>
/// 场合
/// </summary>
public enum Occasion
{
    工作 = 1,
    运动 = 2,
    休闲 = 3,
    正式 = 4,
    约会 = 5,
    面试 = 6,
    健身 = 7
}