using Horolezeni.Models;
using Horolezeni.ViewModels;

namespace Horolezeni.Views
{
    public partial class InfoPage : ContentPage
    {
        public InfoPage(InfoViewModel model)
        {
            BindingContext = model;
            InitializeComponent();
        }
    }
}