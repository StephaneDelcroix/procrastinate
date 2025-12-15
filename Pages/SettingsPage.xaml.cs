namespace procrastinate.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        LoadSettings();
    }

    private void LoadSettings()
    {
        var isHighContrast = Preferences.Get("HighContrastMode", false);
        HighContrastSwitch.IsToggled = isHighContrast;
        UpdatePreview(isHighContrast);
    }

    private void OnHighContrastToggled(object? sender, ToggledEventArgs e)
    {
        Preferences.Set("HighContrastMode", e.Value);
        UpdatePreview(e.Value);
        ApplyTheme(e.Value);
    }

    private void UpdatePreview(bool highContrast)
    {
        if (highContrast)
        {
            PreviewPrimary.BackgroundColor = Color.FromArgb("#FFFF00");
            PreviewSecondary.BackgroundColor = Color.FromArgb("#00FFFF");
            PreviewTertiary.BackgroundColor = Color.FromArgb("#FF00FF");
            PreviewAccent.BackgroundColor = Color.FromArgb("#00FF00");
            ThemeLabel.Text = "Current: High Contrast Theme";
        }
        else
        {
            PreviewPrimary.BackgroundColor = Color.FromArgb("#F59E0B");
            PreviewSecondary.BackgroundColor = Color.FromArgb("#14B8A6");
            PreviewTertiary.BackgroundColor = Color.FromArgb("#8B5CF6");
            PreviewAccent.BackgroundColor = Color.FromArgb("#5EEAD4");
            ThemeLabel.Text = "Current: Default Theme";
        }
    }

    private void ApplyTheme(bool highContrast)
    {
        var resources = Application.Current?.Resources;
        if (resources == null) return;

        if (highContrast)
        {
            // High contrast: bright colors on dark background
            resources["Primary"] = Color.FromArgb("#FFFF00");           // Yellow
            resources["PrimaryDark"] = Color.FromArgb("#FFFF66");
            resources["Secondary"] = Color.FromArgb("#00FFFF");          // Cyan
            resources["SecondaryDarkText"] = Color.FromArgb("#00FFFF");
            resources["Tertiary"] = Color.FromArgb("#FF00FF");           // Magenta
            resources["AccentLight"] = Color.FromArgb("#00FF00");        // Green
            resources["Accent"] = Color.FromArgb("#00FF00");
            resources["CardBackground"] = Color.FromArgb("#000000");     // Pure black
            resources["CardBackgroundAlt"] = Color.FromArgb("#1A1A1A");
            resources["SurfaceBackground"] = Color.FromArgb("#000000");
            resources["Gray100"] = Color.FromArgb("#FFFFFF");
            resources["Gray200"] = Color.FromArgb("#FFFFFF");
            resources["Gray300"] = Color.FromArgb("#FFFFFF");
            resources["Gray400"] = Color.FromArgb("#CCCCCC");
            resources["Warm"] = Color.FromArgb("#FFFF00");
        }
        else
        {
            // Default theme
            resources["Primary"] = Color.FromArgb("#F59E0B");
            resources["PrimaryDark"] = Color.FromArgb("#FBBF24");
            resources["Secondary"] = Color.FromArgb("#14B8A6");
            resources["SecondaryDarkText"] = Color.FromArgb("#5EEAD4");
            resources["Tertiary"] = Color.FromArgb("#8B5CF6");
            resources["AccentLight"] = Color.FromArgb("#5EEAD4");
            resources["Accent"] = Color.FromArgb("#14B8A6");
            resources["CardBackground"] = Color.FromArgb("#1E293B");
            resources["CardBackgroundAlt"] = Color.FromArgb("#334155");
            resources["SurfaceBackground"] = Color.FromArgb("#0F172A");
            resources["Gray100"] = Color.FromArgb("#F1F5F9");
            resources["Gray200"] = Color.FromArgb("#E2E8F0");
            resources["Gray300"] = Color.FromArgb("#CBD5E1");
            resources["Gray400"] = Color.FromArgb("#94A3B8");
            resources["Warm"] = Color.FromArgb("#FEF3C7");
        }
    }
}
