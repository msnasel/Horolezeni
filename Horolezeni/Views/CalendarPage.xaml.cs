using Horolezeni.Models;
using Horolezeni.ViewModels;

namespace Horolezeni.Views
{
    public partial class CalendarPage : ContentPage
    {
        public CalendarPage(CalendarViewModel model)
        {
            BindingContext = model;
            InitializeComponent();
        }
    }
}