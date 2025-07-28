using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DaDaPlace.Services;
using DaDaPlace.Data;
using Microsoft.Extensions.DependencyInjection;

namespace DaDaPlace.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IServiceProvider _serviceProvider;
    
    [ObservableProperty]
    private int selectedTabIndex = 0;
    
    [ObservableProperty]
    private string title = "搭搭星球";
    
    [ObservableProperty]
    private bool isDrawerOpen = false;
    
    [ObservableProperty]
    private WardrobeViewModel? wardrobeViewModel;
    
    [ObservableProperty]
    private OutfitViewModel? outfitViewModel;
    
    [ObservableProperty]
    private CommunityViewModel? communityViewModel;
    
    [ObservableProperty]
    private ProfileViewModel? profileViewModel;
    
    public ObservableCollection<TabItemViewModel> TabItems { get; } = new()
    {
        new TabItemViewModel { Title = "衣橱", Icon = "Shirt" },
        new TabItemViewModel { Title = "搭配", Icon = "Palette" },
        new TabItemViewModel { Title = "社区", Icon = "Users" },
        new TabItemViewModel { Title = "我的", Icon = "User" }
    };
    
    public MainWindowViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeViewModels();
    }
    
    private void InitializeViewModels()
    {
        WardrobeViewModel = _serviceProvider.GetRequiredService<WardrobeViewModel>();
        OutfitViewModel = _serviceProvider.GetRequiredService<OutfitViewModel>();
        CommunityViewModel = _serviceProvider.GetRequiredService<CommunityViewModel>();
        ProfileViewModel = _serviceProvider.GetRequiredService<ProfileViewModel>();
    }
    
    [RelayCommand]
    private void ToggleDrawer()
    {
        IsDrawerOpen = !IsDrawerOpen;
    }
    
    [RelayCommand]
    private void SelectTab(int index)
    {
        SelectedTabIndex = index;
        IsDrawerOpen = false;
    }
    
    [RelayCommand]
    private void AddNewClothing()
    {
        // TODO: 实现添加新衣物功能
        // 可以导航到衣橱页面并触发添加功能
        SelectedTabIndex = 0; // 切换到衣橱页面
        WardrobeViewModel?.AddNewClothingCommand.Execute(null);
    }
    
    [RelayCommand]
    private void GlobalSearch()
    {
        // TODO: 实现全局搜索功能
    }
}

public class TabItemViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}