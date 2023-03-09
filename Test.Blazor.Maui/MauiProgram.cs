using Microsoft.Extensions.Logging;
using Test.Blazor.Client.AppConfig;
using Test.Blazor.Maui.Data;

namespace Test.Blazor.Maui
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

            var bootstrapper = new Bootstrapper(builder);
            bootstrapper.RegisterDependencies();

            return builder.Build();
        }
    }
}