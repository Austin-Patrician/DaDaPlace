using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DaDaPlace.Models;

/// <summary>
/// 社区帖子数据模型
/// </summary>
public class CommunityPost
{
    [Key]
    public int PostId { get; set; }
    
    /// <summary>
    /// 发布者用户ID
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// 发布者用户信息
    /// </summary>
    public UserProfile? User { get; set; }
    
    /// <summary>
    /// 帖子内容
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// 搭配图片URI列表
    /// </summary>
    public List<string> ImageUris { get; set; } = new();
    
    /// <summary>
    /// 关联的搭配方案ID
    /// </summary>
    public int? OutfitLookId { get; set; }
    
    /// <summary>
    /// 关联的搭配方案
    /// </summary>
    public OutfitLook? OutfitLook { get; set; }
    
    /// <summary>
    /// 标签的服装单品信息
    /// </summary>
    public List<TaggedClothingItem> TaggedItems { get; set; } = new();
    
    /// <summary>
    /// 话题标签
    /// </summary>
    public List<string> Topics { get; set; } = new();
    
    /// <summary>
    /// 点赞数
    /// </summary>
    public int LikeCount { get; set; }
    
    /// <summary>
    /// 收藏数
    /// </summary>
    public int FavoriteCount { get; set; }
    
    /// <summary>
    /// 评论数
    /// </summary>
    public int CommentCount { get; set; }
    
    /// <summary>
    /// 分享数
    /// </summary>
    public int ShareCount { get; set; }
    
    /// <summary>
    /// 审核状态
    /// </summary>
    public PostStatus Status { get; set; } = PostStatus.待审核;
    
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
/// 标签的服装单品
/// </summary>
public class TaggedClothingItem
{
    /// <summary>
    /// 服装单品ID
    /// </summary>
    public int ClothingItemId { get; set; }
    
    /// <summary>
    /// 服装单品信息
    /// </summary>
    public ClothingItem? ClothingItem { get; set; }
    
    /// <summary>
    /// 在图片中的X坐标位置(百分比)
    /// </summary>
    public double PositionX { get; set; }
    
    /// <summary>
    /// 在图片中的Y坐标位置(百分比)
    /// </summary>
    public double PositionY { get; set; }
}

/// <summary>
/// 帖子状态
/// </summary>
public enum PostStatus
{
    待审核 = 0,
    已发布 = 1,
    已隐藏 = 2,
    已删除 = 3
}

/// <summary>
/// 评论数据模型
/// </summary>
public class Comment
{
    [Key]
    public int CommentId { get; set; }
    
    /// <summary>
    /// 帖子ID
    /// </summary>
    public int PostId { get; set; }
    
    /// <summary>
    /// 评论者用户ID
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// 评论者用户信息
    /// </summary>
    public UserProfile? User { get; set; }
    
    /// <summary>
    /// 评论内容
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// 父评论ID（回复）
    /// </summary>
    public int? ParentCommentId { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}