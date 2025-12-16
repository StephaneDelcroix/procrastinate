using procrastinate.Pages;

namespace procrastinate;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
		
		// Close settings page when tab changes
		Navigated += OnShellNavigated;
	}

	private async void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
	{
		// If we navigated to a root tab, pop any pages on the stack
		if (e.Current?.Location?.OriginalString?.StartsWith("//") == true)
		{
			try
			{
				var nav = Current?.CurrentPage?.Navigation;
				if (nav?.NavigationStack?.Count > 1)
				{
					await nav.PopToRootAsync(false);
				}
			}
			catch
			{
				// Ignore navigation errors
			}
		}
	}
}
