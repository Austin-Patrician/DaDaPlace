using Avalonia.Controls;
using DaDaPlace.ViewModels;

namespace DaDaPlace.Views;

public partial class OutfitView : UserControl
{
    public OutfitView()
    {
        InitializeComponent();
    }
    
    public OutfitView(OutfitViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}