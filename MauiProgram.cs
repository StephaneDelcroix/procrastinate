using Microsoft.Extensions.Logging;
using procrastinate.Pages;
using procrastinate.Services;

namespace procrastinate;

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

		// Services
		builder.Services.AddSingleton<StatsService>();
		
		// Pages
		builder.Services.AddTransient<TasksPage>();
		builder.Services.AddTransient<GamesPage>();
		builder.Services.AddTransient<ExcusePage>();
		builder.Services.AddTransient<StatsPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
