using procrastinate.Pages;

namespace procrastinate;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
	}

	protected override void OnNavigating(ShellNavigatingEventArgs args)
	{
		base.OnNavigating(args);
		
		// If navigating to a root tab (starts with //), pop any modal pages
		if (args.Target?.Location?.OriginalString?.StartsWith("//") == true)
		{
			// Use MainThread to ensure we pop after navigation completes
			MainThread.BeginInvokeOnMainThread(async () =>
			{
				var nav = Current?.CurrentPage?.Navigation;
				if (nav?.NavigationStack?.Count > 1)
				{
					await nav.PopToRootAsync(false);
				}
			});
		}
	}
}
