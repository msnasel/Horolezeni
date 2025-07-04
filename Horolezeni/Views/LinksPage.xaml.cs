using Horolezeni.Models;
using Horolezeni.ViewModels;

namespace Horolezeni.Views
{
    public partial class LinksPage : ContentPage
    {
        public LinksPage(InfoViewModel model)
        {
            BindingContext = model;
            InitializeComponent();
        }
    }
}