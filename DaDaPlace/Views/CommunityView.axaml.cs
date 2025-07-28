using Avalonia.Controls;
using DaDaPlace.ViewModels;

namespace DaDaPlace.Views;

public partial class CommunityView : UserControl
{
    public CommunityView()
    {
        InitializeComponent();
    }
    
    public CommunityView(CommunityViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}