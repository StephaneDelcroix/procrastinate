using Microsoft.Extensions.DependencyInjection;

namespace procrastinate;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		
		// Apply saved theme preference
		var isLightTheme = Preferences.Get("HighContrastMode", false);
		UserAppTheme = isLightTheme ? AppTheme.Light : AppTheme.Dark;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}