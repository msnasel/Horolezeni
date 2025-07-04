using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using Firebase.Database;
using Horolezeni.Views;
using Horolezeni.ViewModels;
using Horolezeni.Services;
using Horolezeni.Converters;
using System.Reactive.Linq;

namespace Horolezeni
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionToolkit()
                .ConfigureMauiHandlers(handlers =>
                {
#if IOS || MACCATALYST
    				handlers.AddHandler<Microsoft.Maui.Controls.CollectionView, Microsoft.Maui.Controls.Handlers.Items2.CollectionViewHandler2>();
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                    fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
                });

#if DEBUG
    		builder.Logging.AddDebug();
    		builder.Services.AddLogging(configure => configure.AddDebug());
#endif
            builder.Services.AddSingleton<PhotosViewModel>();
            builder.Services.AddSingleton<CalendarViewModel>();
            builder.Services.AddSingleton<LinksViewModel>();
            builder.Services.AddSingleton<NewsViewModel>();
            builder.Services.AddSingleton<ChatViewModel>();
            builder.Services.AddSingleton<InfoViewModel>();
            builder.Services.AddSingleton<StatusToColorConverter>();

            // Registrace služeb
            builder.Services.AddSingleton<FirebaseService>();
            builder.Services.AddSingleton<StatusToColorConverter>();
            builder.Services.AddSingleton<IsNullOrEmptyConverter>();

            // Registrace ViewModels
            builder.Services.AddTransient<AkceViewModel>();
            builder.Services.AddTransient<AkceDetailViewModel>();
            builder.Services.AddTransient<AkceFormViewModel>(); 

            // Registrace Views
            builder.Services.AddTransient<AkcePage>();
            builder.Services.AddTransient<AkceDetailPage>();
            builder.Services.AddTransient<AkceFormPage>();
            builder.Services.AddTransient<LoginPage>();


            // Registrace routování
            Routing.RegisterRoute(nameof(AkceDetailPage), typeof(AkceDetailPage));
            Routing.RegisterRoute(nameof(AkceFormPage), typeof(AkceFormPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            return builder.Build();
        }
    }
}
