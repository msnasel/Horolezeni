using Horolezeni.ViewModels;

namespace Horolezeni.Views;

public partial class AkceDetailPage : ContentPage
{
    public AkceDetailPage(AkceDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
