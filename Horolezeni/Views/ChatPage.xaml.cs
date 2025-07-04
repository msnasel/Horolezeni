using Horolezeni.Models;
using Horolezeni.ViewModels;

namespace Horolezeni.Views
{
    public partial class ChatPage : ContentPage
    {
        public ChatPage(ChatViewModel model)
        {
            BindingContext = model;
            InitializeComponent();
        }
    }
}