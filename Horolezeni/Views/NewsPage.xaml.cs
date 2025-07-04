using Horolezeni.Models;
using Horolezeni.ViewModels;

namespace Horolezeni.Views
{
    public partial class NewsPage : ContentPage
    {
        public NewsPage(NewsViewModel model)
        {
            BindingContext = model;
            InitializeComponent();
        }
    }
}