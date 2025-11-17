using Microsoft.Extensions.Logging;
using MAUI_Minesweeper.ViewModels;
using MAUI_Minesweeper.Views;
using MAUI_Minesweeper.Converters;

namespace MAUI_Minesweeper
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register ViewModels and Pages
            builder.Services.AddSingleton<GameViewModel>();
            builder.Services.AddSingleton<GamePage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
