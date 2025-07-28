using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DaDaPlace.Models;
using Microsoft.Extensions.Logging;

namespace DaDaPlace.ViewModels;

/// <summary>
/// 个人资料页面ViewModel
/// </summary>
public partial class ProfileViewModel : ViewModelBase
{
    private readonly ILogger<ProfileViewModel>? _logger;
    
    [ObservableProperty]
    private UserProfile _userProfile = new();
    
    [ObservableProperty]
    private bool _isEditMode = false;
    
    [ObservableProperty]
    private bool _isLoading = false;
    
    [ObservableProperty]
    private ObservableCollection<SettingItemViewModel> _settingItems = new();
    
    [ObservableProperty]
    private ObservableCollection<CommunityPost> _myPosts = new();
    
    [ObservableProperty]
    private ObservableCollection<OutfitLook> _myOutfits = new();
    
    [ObservableProperty]
    private string _selectedTab = "设置"; // "设置", "我的帖子", "我的搭配"
    
    [ObservableProperty]
    private bool _isDarkTheme = false;
    
    [ObservableProperty]
    private bool _isHighQualityMode = true;
    
    [ObservableProperty]
    private string _selectedTheme = "默认";
    
    public ObservableCollection<string> TabOptions { get; }
    public ObservableCollection<Gender> GenderOptions { get; }
    public ObservableCollection<SkinTone> SkinToneOptions { get; }
    public ObservableCollection<StylePreference> StyleOptions { get; }
    public ObservableCollection<string> ThemeOptions { get; }
    
    public ProfileViewModel(ILogger<ProfileViewModel>? logger = null)
    {
        _logger = logger;
        
        TabOptions = new ObservableCollection<string>
        {
            "设置", "我的帖子", "我的搭配"
        };
        
        GenderOptions = new ObservableCollection<Gender>(Enum.GetValues<Gender>());
        SkinToneOptions = new ObservableCollection<SkinTone>(Enum.GetValues<SkinTone>());
        StyleOptions = new ObservableCollection<StylePreference>(Enum.GetValues<StylePreference>());
        
        ThemeOptions = new ObservableCollection<string>
        {
            "默认", "暗黑", "极光"
        };
        
        InitializeSettingItems();
        _ = LoadUserProfileAsync();
    }
    
    partial void OnSelectedTabChanged(string value)
    {
        switch (value)
        {
            case "我的帖子":
                _ = LoadMyPostsAsync();
                break;
            case "我的搭配":
                _ = LoadMyOutfitsAsync();
                break;
        }
    }
    
    private void InitializeSettingItems()
    {
        SettingItems.Clear();
        
        // 我的资料
        SettingItems.Add(new SettingItemViewModel
        {
            Title = "我的资料",
            Icon = "Person",
            Description = "头像、昵称、身体数据",
            Category = "profile",
            Action = () => ToggleEditMode()
        });
        
        // 账号安全
        SettingItems.Add(new SettingItemViewModel
        {
            Title = "账号安全",
            Icon = "Shield",
            Description = "登录方式、数据备份",
            Category = "security",
            Action = () => OpenAccountSecurity()
        });
        
        // 主题设置
        SettingItems.Add(new SettingItemViewModel
        {
            Title = "主题设置",
            Icon = "Palette",
            Description = "暗黑模式、主题色彩",
            Category = "theme",
            Action = () => OpenThemeSettings()
        });
        
        // 图片质量
        SettingItems.Add(new SettingItemViewModel
        {
            Title = "图片质量",
            Icon = "Image",
            Description = IsHighQualityMode ? "高清模式" : "省流模式",
            Category = "quality",
            Action = () => ToggleImageQuality()
        });
        
        // 帮助与反馈
        SettingItems.Add(new SettingItemViewModel
        {
            Title = "帮助与反馈",
            Icon = "Help",
            Description = "FAQ、问题反馈",
            Category = "help",
            Action = () => OpenHelpAndFeedback()
        });
        
        // 隐私协议
        SettingItems.Add(new SettingItemViewModel
        {
            Title = "隐私协议",
            Icon = "Document",
            Description = "用户协议、隐私政策",
            Category = "privacy",
            Action = () => OpenPrivacyPolicy()
        });
        
        // 数据管理
        SettingItems.Add(new SettingItemViewModel
        {
            Title = "数据管理",
            Icon = "Database",
            Description = "清空数据、账户注销",
            Category = "data",
            Action = () => OpenDataManagement()
        });
    }
    
    [RelayCommand]
    private async Task LoadUserProfileAsync()
    {
        try
        {
            IsLoading = true;
            
            // TODO: 从服务获取用户资料
            // 这里先用模拟数据
            UserProfile = new UserProfile
            {
                UserId = 1,
                Avatar = "/Assets/default_avatar.png",
                Nickname = "时尚达人",
                Gender = Gender.女,
                Height = 165,
                Weight = 50,
                SkinTone = SkinTone.中等,
                StylePreference = StylePreference.休闲,
                CreatedAt = DateTime.Now.AddMonths(-6)
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "加载用户资料失败");
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    private async Task LoadMyPostsAsync()
    {
        try
        {
            // TODO: 加载用户发布的帖子
            MyPosts.Clear();
            _logger?.LogInformation("加载我的帖子");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "加载我的帖子失败");
        }
    }
    
    [RelayCommand]
    private async Task LoadMyOutfitsAsync()
    {
        try
        {
            // TODO: 加载用户保存的搭配
            MyOutfits.Clear();
            _logger?.LogInformation("加载我的搭配");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "加载我的搭配失败");
        }
    }
    
    [RelayCommand]
    private void ToggleEditMode()
    {
        IsEditMode = !IsEditMode;
    }
    
    [RelayCommand]
    private async Task SaveProfileAsync()
    {
        try
        {
            // TODO: 保存用户资料到服务器
            IsEditMode = false;
            _logger?.LogInformation("用户资料已保存");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "保存用户资料失败");
        }
    }
    
    [RelayCommand]
    private void CancelEdit()
    {
        IsEditMode = false;
        // TODO: 恢复原始数据
    }
    
    [RelayCommand]
    private async Task ChangeAvatarAsync()
    {
        try
        {
            // TODO: 打开文件选择器选择头像
            _logger?.LogInformation("更换头像");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "更换头像失败");
        }
    }
    
    [RelayCommand]
    private void OpenAccountSecurity()
    {
        // TODO: 打开账号安全页面
        _logger?.LogInformation("打开账号安全设置");
    }
    
    [RelayCommand]
    private void OpenThemeSettings()
    {
        // TODO: 打开主题设置对话框
        _logger?.LogInformation("打开主题设置");
    }
    
    [RelayCommand]
    private void ToggleImageQuality()
    {
        IsHighQualityMode = !IsHighQualityMode;
        
        // 更新设置项描述
        var qualityItem = SettingItems.FirstOrDefault(x => x.Category == "quality");
        if (qualityItem != null)
        {
            qualityItem.Description = IsHighQualityMode ? "高清模式" : "省流模式";
        }
        
        _logger?.LogInformation($"图片质量模式切换为: {(IsHighQualityMode ? "高清" : "省流")}");
    }
    
    [RelayCommand]
    private void OpenHelpAndFeedback()
    {
        // TODO: 打开帮助与反馈页面
        _logger?.LogInformation("打开帮助与反馈");
    }
    
    [RelayCommand]
    private void OpenPrivacyPolicy()
    {
        // TODO: 打开隐私协议页面
        _logger?.LogInformation("打开隐私协议");
    }
    
    [RelayCommand]
    private void OpenDataManagement()
    {
        // TODO: 打开数据管理页面
        _logger?.LogInformation("打开数据管理");
    }
    
    [RelayCommand]
    private async Task BackupDataAsync()
    {
        try
        {
            // TODO: 备份数据到云端
            _logger?.LogInformation("开始备份数据");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "备份数据失败");
        }
    }
    
    [RelayCommand]
    private async Task ClearDataAsync()
    {
        try
        {
            // TODO: 显示确认对话框，然后清空本地数据
            _logger?.LogInformation("清空本地数据");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "清空数据失败");
        }
    }
    
    [RelayCommand]
    private async Task DeleteAccountAsync()
    {
        try
        {
            // TODO: 显示确认对话框，然后注销账户
            _logger?.LogInformation("注销账户");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "注销账户失败");
        }
    }
    
    [RelayCommand]
    private void SubmitFeedback()
    {
        try
        {
            // TODO: 提交反馈到服务器
            _logger?.LogInformation("提交反馈");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "提交反馈失败");
        }
    }
    
    [RelayCommand]
    private void ChangeTheme(string theme)
    {
        SelectedTheme = theme;
        
        switch (theme)
        {
            case "暗黑":
                IsDarkTheme = true;
                break;
            case "极光":
                // TODO: 应用极光主题
                break;
            default:
                IsDarkTheme = false;
                break;
        }
        
        _logger?.LogInformation($"切换主题为: {theme}");
    }
    
    [RelayCommand]
    private void ViewPost(CommunityPost post)
    {
        // TODO: 查看帖子详情
        _logger?.LogInformation($"查看帖子: {post.PostId}");
    }
    
    [RelayCommand]
    private void ViewOutfit(OutfitLook outfit)
    {
        // TODO: 查看搭配详情
        _logger?.LogInformation($"查看搭配: {outfit.LookId}");
    }
}

/// <summary>
/// 设置项ViewModel
/// </summary>
public partial class SettingItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = string.Empty;
    
    [ObservableProperty]
    private string _icon = string.Empty;
    
    [ObservableProperty]
    private string _description = string.Empty;
    
    [ObservableProperty]
    private string _category = string.Empty;
    
    [ObservableProperty]
    private bool _hasSwitch = false;
    
    [ObservableProperty]
    private bool _switchValue = false;
    
    [ObservableProperty]
    private bool _hasNavigation = true;
    
    public Action? Action { get; set; }
    
    [RelayCommand]
    private void Execute()
    {
        Action?.Invoke();
    }
}