using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using DaDaPlace.Models;
using System.Text.Json;

namespace DaDaPlace.Data;

/// <summary>
/// 应用数据库上下文
/// </summary>
public class AppDbContext : DbContext
{
    public DbSet<ClothingItem> ClothingItems { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<OutfitLook> OutfitLooks { get; set; }
    public DbSet<CommunityPost> CommunityPosts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DaDaPlace", "dadaplace.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // 配置ClothingItem
        modelBuilder.Entity<ClothingItem>(entity =>
        {
            entity.HasKey(e => e.ItemId);
            entity.Property(e => e.SeasonTags)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<Season>>(v, (JsonSerializerOptions?)null) ?? new List<Season>());
            entity.Property(e => e.Occasions)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<Occasion>>(v, (JsonSerializerOptions?)null) ?? new List<Occasion>());
        });
        
        // 配置UserProfile
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.UserId);
        });
        
        // 配置OutfitLook
        modelBuilder.Entity<OutfitLook>(entity =>
        {
            entity.HasKey(e => e.LookId);
            entity.Property(e => e.ClothingItemIds)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions?)null) ?? new List<int>());
            entity.Property(e => e.Occasions)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<Occasion>>(v, (JsonSerializerOptions?)null) ?? new List<Occasion>());
        });
        
        // 配置CommunityPost
        modelBuilder.Entity<CommunityPost>(entity =>
        {
            entity.HasKey(e => e.PostId);
            entity.Property(e => e.ImageUris)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());
            entity.Property(e => e.TaggedItems)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<TaggedClothingItem>>(v, (JsonSerializerOptions?)null) ?? new List<TaggedClothingItem>());
            entity.Property(e => e.Topics)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());
            
            // 配置关系
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.OutfitLook)
                .WithMany()
                .HasForeignKey(e => e.OutfitLookId)
                .OnDelete(DeleteBehavior.SetNull);
        });
        
        // 配置Comment
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId);
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}