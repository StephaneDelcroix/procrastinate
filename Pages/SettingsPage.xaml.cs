using procrastinate.Resources.Strings;
using procrastinate.Services;

namespace procrastinate.Pages;

public partial class SettingsPage : ContentPage
{
    private static readonly string[] GroqModels = 
    {
        "llama-3.3-70b-versatile",
        "llama-3.1-8b-instant",
        "llama-3.2-1b-preview",
        "llama-3.2-3b-preview",
        "mixtral-8x7b-32768",
        "gemma2-9b-it"
    };

    public SettingsPage()
    {
        InitializeComponent();
        LoadSettings();
    }

    private void LoadSettings()
    {
        // Load language picker
        foreach (var lang in AppStrings.SupportedLanguages)
            LanguagePicker.Items.Add(lang.Value);
        
        var savedLang = Preferences.Get("AppLanguage", "");
        var langIndex = AppStrings.SupportedLanguages.Keys.ToList().IndexOf(savedLang);
        LanguagePicker.SelectedIndex = langIndex >= 0 ? langIndex : 0;

        // Load high contrast
        var isHighContrast = Preferences.Get("HighContrastMode", false);
        HighContrastSwitch.IsToggled = isHighContrast;
        UpdatePreview(isHighContrast);
        UpdateThemeLabel(isHighContrast);

        // Load Zalgo mode
        ZalgoSwitch.IsToggled = AppStrings.IsZalgoMode;

        // Load excuse engine settings
        foreach (var mode in ExcuseService.AvailableModes)
            ExcuseModePicker.Items.Add(mode.Value);
        
        var currentMode = ExcuseService.CurrentMode;
        var modeIndex = ExcuseService.AvailableModes.Keys.ToList().IndexOf(currentMode);
        ExcuseModePicker.SelectedIndex = modeIndex >= 0 ? modeIndex : 0;

        GroqApiKeyEntry.Text = Preferences.Get("GroqApiKey", "");
        
        // Load Groq models picker
        foreach (var model in GroqModels)
            GroqModelPicker.Items.Add(model);
        
        var savedModel = Preferences.Get("GroqModel", "llama-3.3-70b-versatile");
        var modelIndex = Array.IndexOf(GroqModels, savedModel);
        GroqModelPicker.SelectedIndex = modelIndex >= 0 ? modelIndex : 0;
        
        UpdateAISettingsVisibility();

        // Display version
        var version = AppInfo.VersionString;
        var build = AppInfo.BuildString;
        VersionLabel.Text = $"Procrastinate v{version} (build {build})";
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        if (LanguagePicker.SelectedIndex < 0) return;
        
        var langCode = AppStrings.SupportedLanguages.Keys.ElementAt(LanguagePicker.SelectedIndex);
        AppStrings.CurrentLanguage = langCode;
        UpdateThemeLabel(Preferences.Get("HighContrastMode", false));
    }

    private void UpdateThemeLabel(bool isHighContrast)
    {
        var themeName = isHighContrast ? AppStrings.GetString("HighContrast") : AppStrings.GetString("DefaultTheme");
        ThemeLabel.Text = AppStrings.GetString("CurrentTheme", themeName);
    }

    private void OnHighContrastToggled(object? sender, ToggledEventArgs e)
    {
        Preferences.Set("HighContrastMode", e.Value);
        UpdatePreview(e.Value);
        ApplyTheme(e.Value);
        UpdateThemeLabel(e.Value);
    }

    private void OnZalgoToggled(object? sender, ToggledEventArgs e)
    {
        AppStrings.IsZalgoMode = e.Value;
    }

    private void OnExcuseModeChanged(object? sender, EventArgs e)
    {
        if (ExcuseModePicker.SelectedIndex < 0) return;
        
        var modeKey = ExcuseService.AvailableModes.Keys.ElementAt(ExcuseModePicker.SelectedIndex);
        ExcuseService.CurrentMode = modeKey;
        UpdateAISettingsVisibility();
    }

    private void UpdateAISettingsVisibility()
    {
        CloudSettingsPanel.IsVisible = ExcuseService.CurrentMode == "cloud";
        OnDeviceAISettingsPanel.IsVisible = ExcuseService.CurrentMode == "ondevice";
        
        if (ExcuseService.CurrentMode == "ondevice")
        {
            UpdateOnDeviceAIStatus();
        }
    }

    private void UpdateOnDeviceAIStatus()
    {
        var generator = new OnDeviceAIExcuseGenerator();
        if (generator.IsAvailable)
        {
            OnDeviceAIStatusLabel.Text = AppStrings.Instance.OnDeviceAIAvailable;
            OnDeviceAIStatusLabel.TextColor = Color.FromArgb("#14B8A6"); // Secondary/teal
        }
        else
        {
            OnDeviceAIStatusLabel.Text = AppStrings.Instance.OnDeviceAIUnavailable;
            OnDeviceAIStatusLabel.TextColor = Color.FromArgb("#F59E0B"); // Primary/amber warning
        }
    }

    private void OnGroqApiKeyChanged(object? sender, TextChangedEventArgs e)
    {
        Preferences.Set("GroqApiKey", e.NewTextValue ?? "");
    }

    private void OnGroqModelChanged(object? sender, EventArgs e)
    {
        if (GroqModelPicker.SelectedIndex < 0) return;
        var model = GroqModels[GroqModelPicker.SelectedIndex];
        Preferences.Set("GroqModel", model);
    }

    private void UpdatePreview(bool highContrast)
    {
        if (highContrast)
        {
            // Nord Light preview
            PreviewPrimary.BackgroundColor = Color.FromArgb("#5E81AC");
            PreviewSecondary.BackgroundColor = Color.FromArgb("#81A1C1");
            PreviewTertiary.BackgroundColor = Color.FromArgb("#88C0D0");
            PreviewAccent.BackgroundColor = Color.FromArgb("#A3BE8C");
        }
        else
        {
            // Nord Dark preview
            PreviewPrimary.BackgroundColor = Color.FromArgb("#88C0D0");
            PreviewSecondary.BackgroundColor = Color.FromArgb("#81A1C1");
            PreviewTertiary.BackgroundColor = Color.FromArgb("#5E81AC");
            PreviewAccent.BackgroundColor = Color.FromArgb("#A3BE8C");
        }
    }

    private void ApplyTheme(bool highContrast)
    {
        var resources = Application.Current?.Resources;
        if (resources == null) return;

        if (highContrast)
        {
            // Nord Light Theme - Snow Storm backgrounds with Frost accents
            resources["Primary"] = Color.FromArgb("#5E81AC");
            resources["PrimaryDark"] = Color.FromArgb("#81A1C1");
            resources["Secondary"] = Color.FromArgb("#5E81AC");
            resources["SecondaryDarkText"] = Color.FromArgb("#5E81AC");
            resources["Tertiary"] = Color.FromArgb("#5E81AC");
            resources["AccentLight"] = Color.FromArgb("#A3BE8C");
            resources["Accent"] = Color.FromArgb("#A3BE8C");
            resources["CardBackground"] = Color.FromArgb("#E5E9F0");
            resources["CardBackgroundAlt"] = Color.FromArgb("#D8DEE9");
            resources["SurfaceBackground"] = Color.FromArgb("#ECEFF4");
            resources["Gray100"] = Color.FromArgb("#2E3440");
            resources["Gray200"] = Color.FromArgb("#3B4252");
            resources["Gray300"] = Color.FromArgb("#434C5E");
            resources["Gray400"] = Color.FromArgb("#4C566A");
            resources["Warm"] = Color.FromArgb("#D08770");
        }
        else
        {
            // Nord Dark Theme - Polar Night backgrounds with Frost accents
            resources["Primary"] = Color.FromArgb("#88C0D0");
            resources["PrimaryDark"] = Color.FromArgb("#8FBCBB");
            resources["Secondary"] = Color.FromArgb("#81A1C1");
            resources["SecondaryDarkText"] = Color.FromArgb("#88C0D0");
            resources["Tertiary"] = Color.FromArgb("#5E81AC");
            resources["AccentLight"] = Color.FromArgb("#EBCB8B");
            resources["Accent"] = Color.FromArgb("#A3BE8C");
            resources["CardBackground"] = Color.FromArgb("#3B4252");
            resources["CardBackgroundAlt"] = Color.FromArgb("#434C5E");
            resources["SurfaceBackground"] = Color.FromArgb("#2E3440");
            resources["Gray100"] = Color.FromArgb("#ECEFF4");
            resources["Gray200"] = Color.FromArgb("#E5E9F0");
            resources["Gray300"] = Color.FromArgb("#D8DEE9");
            resources["Gray400"] = Color.FromArgb("#D8DEE9");
            resources["Warm"] = Color.FromArgb("#EBCB8B");
        }
    }

    private async void OnGitHubTapped(object? sender, EventArgs e)
    {
        try
        {
            await Launcher.OpenAsync("https://github.com/StephaneDelcroix/procrastinate");
        }
        catch { }
    }
}
