using Horolezeni.ViewModels;

namespace Horolezeni.Views;

public partial class AkceFormPage : ContentPage
{
    public AkceFormPage(AkceFormViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}