﻿using Fb2.Document.MAUI.Blazor.Playground.Data;
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
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            builder.Services
                .AddSingleton<WeatherForecastService>()
                .AddSingleton<AppStateService>();

            return builder.Build();
        }
    }
}