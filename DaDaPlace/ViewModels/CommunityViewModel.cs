using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DaDaPlace.Models;
using Microsoft.Extensions.Logging;

namespace DaDaPlace.ViewModels;

/// <summary>
/// 社区页面ViewModel
/// </summary>
public partial class CommunityViewModel : ViewModelBase
{
    private readonly ILogger<CommunityViewModel>? _logger;
    
    [ObservableProperty]
    private ObservableCollection<CommunityPost> _posts = new();
    
    [ObservableProperty]
    private ObservableCollection<CommunityPost> _filteredPosts = new();
    
    [ObservableProperty]
    private string _selectedFeedType = "综合"; // "综合", "关注", "最新"
    
    [ObservableProperty]
    private bool _isLoading = false;
    
    [ObservableProperty]
    private bool _isRefreshing = false;
    
    [ObservableProperty]
    private CommunityPost? _selectedPost;
    
    [ObservableProperty]
    private string _searchText = string.Empty;
    
    [ObservableProperty]
    private ObservableCollection<string> _trendingTopics = new();
    
    [ObservableProperty]
    private ObservableCollection<WeeklyChallengeViewModel> _weeklyChallenges = new();
    
    [ObservableProperty]
    private bool _isCreatePostDialogOpen = false;
    
    [ObservableProperty]
    private CreatePostViewModel _createPostViewModel = new();
    
    public ObservableCollection<string> FeedTypes { get; }
    
    public CommunityViewModel(ILogger<CommunityViewModel>? logger = null)
    {
        _logger = logger;
        
        FeedTypes = new ObservableCollection<string>
        {
            "综合", "关注", "最新"
        };
        
        // 初始化热门话题
        TrendingTopics = new ObservableCollection<string>
        {
            "#夏日穿搭", "#职场OOTD", "#约会穿搭", "#周末休闲", "#色彩搭配", "#极简风格"
        };
        
        // 初始化每周挑战
        InitializeWeeklyChallenges();
        
        // 加载数据
        _ = LoadPostsAsync();
    }
    
    partial void OnSelectedFeedTypeChanged(string value)
    {
        _ = ApplyFeedFilterAsync();
    }
    
    partial void OnSearchTextChanged(string value)
    {
        _ = ApplySearchFilterAsync();
    }
    
    private void InitializeWeeklyChallenges()
    {
        WeeklyChallenges.Add(new WeeklyChallengeViewModel
        {
            Title = "黑白极简",
            Description = "用黑白两色打造极简风格穿搭",
            ImageUri = "/Assets/challenge_bw.png",
            ParticipantCount = 1234,
            EndDate = DateTime.Now.AddDays(5),
            Reward = "社区VIP滤镜"
        });
        
        WeeklyChallenges.Add(new WeeklyChallengeViewModel
        {
            Title = "夏日Cityboy",
            Description = "展现都市男孩的夏日风采",
            ImageUri = "/Assets/challenge_cityboy.png",
            ParticipantCount = 856,
            EndDate = DateTime.Now.AddDays(3),
            Reward = "专属徽章"
        });
    }
    
    [RelayCommand]
    private async Task LoadPostsAsync()
    {
        try
        {
            IsLoading = true;
            
            // TODO: 从服务获取帖子数据
            // 这里先用模拟数据
            var mockPosts = GenerateMockPosts();
            
            Posts.Clear();
            foreach (var post in mockPosts)
            {
                Posts.Add(post);
            }
            
            await ApplyFeedFilterAsync();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "加载社区帖子失败");
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    private async Task RefreshPostsAsync()
    {
        try
        {
            IsRefreshing = true;
            await LoadPostsAsync();
        }
        finally
        {
            IsRefreshing = false;
        }
    }
    
    [RelayCommand]
    private async Task ApplyFeedFilterAsync()
    {
        try
        {
            var filtered = Posts.AsEnumerable();
            
            switch (SelectedFeedType)
            {
                case "关注":
                    // TODO: 筛选关注用户的帖子
                    filtered = filtered.Where(p => p.UserId == 1); // TODO: 替换为实际的关注用户ID列表
                    break;
                case "最新":
                    filtered = filtered.OrderByDescending(p => p.CreatedAt);
                    break;
                case "综合":
                default:
                    // 综合排序：考虑点赞数、评论数、发布时间等
                    filtered = filtered.OrderByDescending(p => 
                        p.LikeCount * 0.3 + p.CommentCount * 0.5 + 
                        (DateTime.Now - p.CreatedAt).TotalHours * -0.1);
                    break;
            }
            
            FilteredPosts.Clear();
            foreach (var post in filtered)
            {
                FilteredPosts.Add(post);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "应用信息流筛选失败");
        }
    }
    
    [RelayCommand]
    private async Task ApplySearchFilterAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                await ApplyFeedFilterAsync();
                return;
            }
            
            var searchResults = Posts.Where(p => 
                p.Content.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                p.Topics.Any(t => t.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                p.TaggedItems.Any(t => t.ClothingItem.Category2.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
            
            FilteredPosts.Clear();
            foreach (var post in searchResults)
            {
                FilteredPosts.Add(post);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "搜索帖子失败");
        }
    }
    
    [RelayCommand]
    private async Task LikePostAsync(CommunityPost post)
    {
        try
        {
            // TODO: 调用点赞API
            // 简单的点赞逻辑，实际应该从服务器获取状态
            post.LikeCount += 1;
            
            _logger?.LogInformation($"点赞帖子: {post.PostId}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"点赞帖子失败: {post.PostId}");
        }
    }
    
    [RelayCommand]
    private async Task CollectPostAsync(CommunityPost post)
    {
        try
        {
            // TODO: 调用收藏API
            // 简单的收藏状态切换逻辑，实际应该从服务器获取状态
            post.FavoriteCount += 1;
            
            _logger?.LogInformation($"收藏帖子: {post.PostId}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"收藏帖子失败: {post.PostId}");
        }
    }
    
    [RelayCommand]
    private void ViewPostDetail(CommunityPost post)
    {
        SelectedPost = post;
        // TODO: 打开帖子详情页面
    }
    
    [RelayCommand]
    private void ViewUserProfile(int userId)
    {
        // TODO: 打开用户资料页面
        _logger?.LogInformation($"查看用户资料: {userId}");
    }
    
    [RelayCommand]
    private void OpenCreatePost()
    {
        CreatePostViewModel = new CreatePostViewModel();
        IsCreatePostDialogOpen = true;
    }
    
    [RelayCommand]
    private void CloseCreatePost()
    {
        IsCreatePostDialogOpen = false;
    }
    
    [RelayCommand]
    private async Task PublishPostAsync()
    {
        try
        {
            // TODO: 发布帖子到服务器
            var newPost = new CommunityPost
            {
                PostId = new Random().Next(1000, 9999),
                UserId = 1, // TODO: 获取当前用户ID
                Content = CreatePostViewModel.Content,
                ImageUris = CreatePostViewModel.ImageUris.ToList(),
                Topics = CreatePostViewModel.Topics.ToList(),
                CreatedAt = DateTime.Now,
                Status = PostStatus.已发布
            };
            
            Posts.Insert(0, newPost);
            await ApplyFeedFilterAsync();
            
            IsCreatePostDialogOpen = false;
            _logger?.LogInformation($"发布帖子成功: {newPost.PostId}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "发布帖子失败");
        }
    }
    
    [RelayCommand]
    private void TryOnOutfit(CommunityPost post)
    {
        try
        {
            // TODO: 实现"一键试穿"功能
            // 在用户衣橱中找到等价替代品
            _logger?.LogInformation($"尝试试穿帖子中的搭配: {post.PostId}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"试穿搭配失败: {post.PostId}");
        }
    }
    
    [RelayCommand]
    private void JoinChallenge(WeeklyChallengeViewModel challenge)
    {
        try
        {
            // TODO: 参加每周挑战
            challenge.HasJoined = true;
            challenge.ParticipantCount++;
            
            _logger?.LogInformation($"参加挑战: {challenge.Title}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"参加挑战失败: {challenge.Title}");
        }
    }
    
    [RelayCommand]
    private void SearchByTopic(string topic)
    {
        SearchText = topic;
    }
    
    [RelayCommand]
    private async Task LoadMorePostsAsync()
    {
        try
        {
            // TODO: 加载更多帖子（分页）
            _logger?.LogInformation("加载更多帖子");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "加载更多帖子失败");
        }
    }
    
    private List<CommunityPost> GenerateMockPosts()
    {
        // 生成模拟数据
        return new List<CommunityPost>
        {
            new CommunityPost
            {
                PostId = 1,
                UserId = 1,
                Content = "今天的通勤穿搭，简约而不简单 ✨",
                ImageUris = new List<string> { "/Assets/outfit1.jpg" },
                Topics = new List<string> { "#职场OOTD", "#简约风格" },
                LikeCount = 128,
                CommentCount = 23,
                FavoriteCount = 45,
                CreatedAt = DateTime.Now.AddHours(-2),
                Status = PostStatus.已发布
            },
            new CommunityPost
            {
                PostId = 2,
                UserId = 2,
                Content = "周末约会穿搭分享，甜美可爱风 💕",
                ImageUris = new List<string> { "/Assets/outfit2.jpg", "/Assets/outfit2_detail.jpg" },
                Topics = new List<string> { "#约会穿搭", "#甜美风" },
                LikeCount = 256,
                CommentCount = 67,
                FavoriteCount = 89,
                CreatedAt = DateTime.Now.AddHours(-5),
                Status = PostStatus.已发布
            }
        };
    }
}

/// <summary>
/// 每周挑战ViewModel
/// </summary>
public partial class WeeklyChallengeViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = string.Empty;
    
    [ObservableProperty]
    private string _description = string.Empty;
    
    [ObservableProperty]
    private string _imageUri = string.Empty;
    
    [ObservableProperty]
    private int _participantCount;
    
    [ObservableProperty]
    private DateTime _endDate;
    
    [ObservableProperty]
    private string _reward = string.Empty;
    
    [ObservableProperty]
    private bool _hasJoined = false;
    
    public TimeSpan TimeRemaining => EndDate - DateTime.Now;
    public string TimeRemainingText => $"{TimeRemaining.Days}天 {TimeRemaining.Hours}小时";
}

/// <summary>
/// 创建帖子ViewModel
/// </summary>
public partial class CreatePostViewModel : ObservableObject
{
    [ObservableProperty]
    private string _content = string.Empty;
    
    [ObservableProperty]
    private ObservableCollection<string> _imageUris = new();
    
    [ObservableProperty]
    private ObservableCollection<string> _topics = new();
    
    [ObservableProperty]
    private ObservableCollection<TaggedClothingItem> _taggedItems = new();
    
    [ObservableProperty]
    private bool _isPublic = true;
}