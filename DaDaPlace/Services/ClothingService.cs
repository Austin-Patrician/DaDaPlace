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
/// 衣橱管理服务实现
/// </summary>
public class ClothingService : IClothingService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ClothingService> _logger;
    
    public ClothingService(AppDbContext context, ILogger<ClothingService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<ClothingItem> AddClothingItemAsync(ClothingItem item)
    {
        try
        {
            item.CreatedAt = DateTime.Now;
            item.UpdatedAt = DateTime.Now;
            
            _context.ClothingItems.Add(item);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"添加服装单品成功: {item.ItemId}");
            return item;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加服装单品失败");
            throw;
        }
    }
    
    public async Task<List<ClothingItem>> GetAllClothingItemsAsync()
    {
        try
        {
            return await _context.ClothingItems
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取所有服装单品失败");
            throw;
        }
    }
    
    public async Task<ClothingItem?> GetClothingItemByIdAsync(int itemId)
    {
        try
        {
            return await _context.ClothingItems
                .FirstOrDefaultAsync(x => x.ItemId == itemId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"获取服装单品失败: {itemId}");
            throw;
        }
    }
    
    public async Task<ClothingItem> UpdateClothingItemAsync(ClothingItem item)
    {
        try
        {
            item.UpdatedAt = DateTime.Now;
            
            _context.ClothingItems.Update(item);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"更新服装单品成功: {item.ItemId}");
            return item;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"更新服装单品失败: {item.ItemId}");
            throw;
        }
    }
    
    public async Task<bool> DeleteClothingItemAsync(int itemId)
    {
        try
        {
            var item = await _context.ClothingItems.FindAsync(itemId);
            if (item == null)
                return false;
                
            _context.ClothingItems.Remove(item);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"删除服装单品成功: {itemId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"删除服装单品失败: {itemId}");
            throw;
        }
    }
    
    public async Task<List<ClothingItem>> GetClothingItemsByCategoryAsync(ClothingCategory category)
    {
        try
        {
            return await _context.ClothingItems
                .Where(x => x.Category1 == category)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"根据分类获取服装单品失败: {category}");
            throw;
        }
    }
    
    public async Task<List<ClothingItem>> GetClothingItemsBySeasonAsync(Season season)
    {
        try
        {
            return await _context.ClothingItems
                .Where(x => x.SeasonTags.Contains(season))
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"根据季节获取服装单品失败: {season}");
            throw;
        }
    }
    
    public async Task<List<ClothingItem>> GetClothingItemsByOccasionAsync(Occasion occasion)
    {
        try
        {
            return await _context.ClothingItems
                .Where(x => x.Occasions.Contains(occasion))
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"根据场合获取服装单品失败: {occasion}");
            throw;
        }
    }
    
    public async Task<List<ClothingItem>> GetClothingItemsByColorAsync(string color)
    {
        try
        {
            return await _context.ClothingItems
                .Where(x => x.BaseColor.Contains(color))
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"根据颜色获取服装单品失败: {color}");
            throw;
        }
    }
    
    public async Task<List<ClothingItem>> SearchClothingItemsAsync(string keyword)
    {
        try
        {
            return await _context.ClothingItems
                .Where(x => x.Category2.Contains(keyword) || 
                           x.Brand.Contains(keyword) || 
                           x.Note.Contains(keyword) ||
                           x.BaseColor.Contains(keyword))
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"搜索服装单品失败: {keyword}");
            throw;
        }
    }
    
    public async Task<List<ClothingItem>> GetFavoriteClothingItemsAsync()
    {
        try
        {
            return await _context.ClothingItems
                .Where(x => x.IsFavorite)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取收藏服装单品失败");
            throw;
        }
    }
    
    public async Task<bool> ToggleFavoriteAsync(int itemId)
    {
        try
        {
            var item = await _context.ClothingItems.FindAsync(itemId);
            if (item == null)
                return false;
                
            item.IsFavorite = !item.IsFavorite;
            item.UpdatedAt = DateTime.Now;
            
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"切换收藏状态成功: {itemId} -> {item.IsFavorite}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"切换收藏状态失败: {itemId}");
            throw;
        }
    }
    
    public async Task<bool> IncrementWornCountAsync(int itemId)
    {
        try
        {
            var item = await _context.ClothingItems.FindAsync(itemId);
            if (item == null)
                return false;
                
            item.WornCount++;
            item.UpdatedAt = DateTime.Now;
            
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"增加穿着次数成功: {itemId} -> {item.WornCount}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"增加穿着次数失败: {itemId}");
            throw;
        }
    }
    
    public async Task<(ClothingCategory category, string color)> AnalyzeClothingImageAsync(string imagePath)
    {
        // TODO: 实现AI图像识别功能
        // 这里可以集成Google MLKit Vision或其他AI服务
        await Task.Delay(100); // 模拟异步操作
        
        _logger.LogInformation($"分析服装图片: {imagePath}");
        
        // 临时返回默认值，实际应该调用AI服务
        return (ClothingCategory.上衣, "白色");
    }
    
    public async Task<string> CropClothingImageAsync(string imagePath)
    {
        // TODO: 实现图片自动裁剪功能
        // 这里可以使用图像处理库进行自动裁剪和抠图
        await Task.Delay(100); // 模拟异步操作
        
        _logger.LogInformation($"裁剪服装图片: {imagePath}");
        
        // 临时返回原路径，实际应该返回裁剪后的图片路径
        return imagePath;
    }
}