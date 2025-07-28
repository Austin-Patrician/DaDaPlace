using System;
using System.ComponentModel.DataAnnotations;

namespace DaDaPlace.Models;

/// <summary>
/// 用户资料数据模型
/// </summary>
public class UserProfile
{
    [Key]
    public int UserId { get; set; }
    
    /// <summary>
    /// 头像URI
    /// </summary>
    public string Avatar { get; set; } = string.Empty;
    
    /// <summary>
    /// 昵称
    /// </summary>
    public string Nickname { get; set; } = string.Empty;
    
    /// <summary>
    /// 性别
    /// </summary>
    public Gender Gender { get; set; }
    
    /// <summary>
    /// 身高(cm)
    /// </summary>
    public int? Height { get; set; }
    
    /// <summary>
    /// 体重(kg)
    /// </summary>
    public decimal? Weight { get; set; }
    
    /// <summary>
    /// 肤色
    /// </summary>
    public SkinTone SkinTone { get; set; }
    
    /// <summary>
    /// 穿衣风格偏好
    /// </summary>
    public StylePreference StylePreference { get; set; }
    
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
/// 性别
/// </summary>
public enum Gender
{
    未设置 = 0,
    男 = 1,
    女 = 2
}

/// <summary>
/// 肤色
/// </summary>
public enum SkinTone
{
    未设置 = 0,
    浅色 = 1,
    中等 = 2,
    深色 = 3
}

/// <summary>
/// 穿衣风格偏好
/// </summary>
public enum StylePreference
{
    未设置 = 0,
    简约 = 1,
    休闲 = 2,
    商务 = 3,
    时尚 = 4,
    运动 = 5,
    复古 = 6,
    甜美 = 7,
    酷帅 = 8
}