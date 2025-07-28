using Avalonia.Controls;
using DaDaPlace.ViewModels;

namespace DaDaPlace.Views;

public partial class ProfileView : UserControl
{
    public ProfileView()
    {
        InitializeComponent();
    }
    
    public ProfileView(ProfileViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}