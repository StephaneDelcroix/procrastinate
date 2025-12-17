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

    private static readonly string[] ThemeOptions = { "system", "light", "dark" };

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

        // Load theme picker
        ThemePicker.Items.Add("System Default");
        ThemePicker.Items.Add("Light");
        ThemePicker.Items.Add("Dark");
        
        var savedTheme = Preferences.Get("AppTheme", "system");
        var themeIndex = Array.IndexOf(ThemeOptions, savedTheme);
        ThemePicker.SelectedIndex = themeIndex >= 0 ? themeIndex : 0;
        UpdateThemeLabel();
        ApplyTheme(savedTheme);

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
        UpdateThemeLabel();
    }

    private void UpdateThemeLabel()
    {
        var themeIndex = ThemePicker.SelectedIndex >= 0 ? ThemePicker.SelectedIndex : 0;
        var themeName = themeIndex switch
        {
            1 => "Light",
            2 => "Dark",
            _ => "System Default"
        };
        ThemeLabel.Text = $"Current: {themeName}";
    }

    private void OnThemeChanged(object? sender, EventArgs e)
    {
        if (ThemePicker.SelectedIndex < 0) return;
        
        var theme = ThemeOptions[ThemePicker.SelectedIndex];
        Preferences.Set("AppTheme", theme);
        ApplyTheme(theme);
        UpdateThemeLabel();
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
            OnDeviceAIStatusLabel.TextColor = Color.FromArgb("#A3BE8C"); // Nord green
        }
        else
        {
            OnDeviceAIStatusLabel.Text = AppStrings.Instance.OnDeviceAIUnavailable;
            OnDeviceAIStatusLabel.TextColor = Color.FromArgb("#EBCB8B"); // Nord yellow warning
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

    private void ApplyTheme(string theme)
    {
        if (Application.Current == null) return;
        
        Application.Current.UserAppTheme = theme switch
        {
            "light" => AppTheme.Light,
            "dark" => AppTheme.Dark,
            _ => AppTheme.Unspecified // System default
        };
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
