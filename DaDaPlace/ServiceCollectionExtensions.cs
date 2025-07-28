using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using DaDaPlace.Data;
using DaDaPlace.Services;
using DaDaPlace.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DaDaPlace;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDaDaPlaceServices(this IServiceCollection services)
    {
        // 数据库上下文
        services.AddDbContext<AppDbContext>(options =>
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dbPath = Path.Combine(appDataPath, "DaDaPlace", "dadaplace.db");
            options.UseSqlite($"Data Source={dbPath}");
        });
        
        // 服务注册
        services.AddScoped<IClothingService, ClothingService>();
        services.AddScoped<IOutfitService, OutfitService>();
        
        // ViewModels注册
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<WardrobeViewModel>();
        services.AddTransient<OutfitViewModel>();
        services.AddTransient<CommunityViewModel>();
        services.AddTransient<ProfileViewModel>();
        
        return services;
    }
    
    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        // 确保数据库目录存在
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var dbDirectory = Path.Combine(appDataPath, "DaDaPlace");
        Directory.CreateDirectory(dbDirectory);
        
        // 创建数据库
        await context.Database.EnsureCreatedAsync();
    }
}