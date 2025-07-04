using Horolezeni.Models;
using Horolezeni.ViewModels;

namespace Horolezeni.Views
{
    public partial class PhotosPage : ContentPage
    {
        public PhotosPage(PhotosViewModel model)
        {
            BindingContext = model;
            InitializeComponent();
        }
    }
}