using Avalonia.Controls;
using DaDaPlace.ViewModels;

namespace DaDaPlace.Views;

public partial class WardrobeView : UserControl
{
    public WardrobeView()
    {
        InitializeComponent();
    }
    
    public WardrobeView(WardrobeViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}