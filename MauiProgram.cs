using Microsoft.AspNetCore.Components.WebView.Maui;
using DavedecV3.Data;
using Havit.Blazor.Components.Web;

namespace DavedecV3;

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
#endif
		builder.Services.AddHxServices();
		builder.Services.AddHxMessenger();		
		builder.Services.AddSingleton<WeatherForecastService>();
		builder.Services.AddSingleton<DocumentoService>();

		return builder.Build();
	}
}
