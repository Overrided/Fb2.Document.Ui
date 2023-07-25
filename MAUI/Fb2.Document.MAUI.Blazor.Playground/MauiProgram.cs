using CommunityToolkit.Maui;
using Fb2.Document.MAUI.Blazor.Playground.Data;
using Fb2.Document.MAUI.Blazor.Playground.Services;
using Microsoft.Extensions.Logging;

namespace Fb2.Document.MAUI.Blazor.Playground
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                // After initializing the .NET MAUI Community Toolkit, optionally add additional fonts
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            builder.Services
                .AddSingleton<AppStateService>();

            return builder.Build();
        }
    }
}