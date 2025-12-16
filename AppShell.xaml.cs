using procrastinate.Pages;

namespace procrastinate;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
		
		// Close settings page when navigating between tabs
		Navigating += OnShellNavigating;
	}

	private async void OnShellNavigating(object? sender, ShellNavigatingEventArgs e)
	{
		// If navigating to a root tab and settings is on the stack, pop it
		if (e.Target?.Location?.OriginalString?.StartsWith("//") == true)
		{
			var nav = Current?.Navigation;
			if (nav?.NavigationStack?.Count > 1)
			{
				// Pop all pages except the root
				while (nav.NavigationStack.Count > 1)
				{
					await nav.PopAsync(false);
				}
			}
		}
	}
}
