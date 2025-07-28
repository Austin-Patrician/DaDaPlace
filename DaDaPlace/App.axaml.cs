using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DaDaPlace.ViewModels;
using DaDaPlace.Views;

namespace DaDaPlace;

public partial class App : Application
{
    private IHost? _host;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            
            // 配置依赖注入
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddDaDaPlaceServices();
                })
                .Build();
            
            await _host.StartAsync();
            
            // 初始化数据库
            await _host.Services.InitializeDatabaseAsync();
            
            var mainWindowViewModel = _host.Services.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel,
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            DisableAvaloniaDataAnnotationValidation();
            
            // 配置依赖注入
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddDaDaPlaceServices();
                })
                .Build();
            
            await _host.StartAsync();
            
            // 初始化数据库
            await _host.Services.InitializeDatabaseAsync();
            
            var mainWindowViewModel = _host.Services.GetRequiredService<MainWindowViewModel>();
            singleViewPlatform.MainView = new MainWindow
            {
                DataContext = mainWindowViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    // 在应用关闭时清理资源
    // 注意：Avalonia应用的生命周期管理可能需要不同的方法

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}