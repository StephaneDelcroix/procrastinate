using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace procrastinate.Resources.Strings;

public class AppStrings : INotifyPropertyChanged
{
    private static readonly Lazy<AppStrings> _instance = new(() => new AppStrings());
    public static AppStrings Instance => _instance.Value;

    private static readonly ResourceManager _resourceManager = 
        new("procrastinate.Resources.Strings.AppResources", typeof(AppStrings).Assembly);

    private CultureInfo _culture;

    public event PropertyChangedEventHandler? PropertyChanged;

    public static readonly Dictionary<string, string> SupportedLanguages = new()
    {
        { "en", "English" },
        { "fr", "Français" },
        { "es", "Español" },
        { "pt", "Português" },
        { "nl", "Nederlands" },
        { "cs", "Čeština" }
    };

    private AppStrings()
    {
        var savedLang = Preferences.Get("AppLanguage", "en");
        _culture = new CultureInfo(savedLang);
    }

    public static string CurrentLanguage
    {
        get => Instance._culture.TwoLetterISOLanguageName;
        set
        {
            Instance._culture = new CultureInfo(value);
            Preferences.Set("AppLanguage", value);
            Instance.OnPropertyChanged(null); // Notify all properties changed
        }
    }

    public string this[string key] => GetString(key);

    public static string GetString(string key)
    {
        return _resourceManager.GetString(key, Instance._culture) ?? key;
    }

    public static string GetString(string key, params object[] args)
    {
        var format = GetString(key);
        return string.Format(format, args);
    }

    // For backwards compatibility
    public static string Get(string key) => GetString(key);
    public static string Get(string key, params object[] args) => GetString(key, args);

    protected void OnPropertyChanged(string? propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Properties for direct XAML binding
    public string AppName => this["AppName"];
    public string Settings => this["Settings"];
    public string Accessibility => this["Accessibility"];
    public string HighContrastMode => this["HighContrastMode"];
    public string HighContrastDesc => this["HighContrastDesc"];
    public string ThemePreview => this["ThemePreview"];
    public string DefaultTheme => this["DefaultTheme"];
    public string HighContrast => this["HighContrast"];
    public string ChangesApply => this["ChangesApply"];
    public string Language => this["Language"];
    public string TodaysTasks => this["TodaysTasks"];
    public string YourProductivityList => this["YourProductivityList"];
    public string TakeABreak => this["TakeABreak"];
    public string DoingGreat => this["DoingGreat"];
    public string AddMoreTasks => this["AddMoreTasks"];
    public string Congratulations => this["Congratulations"];
    public string NeedAnotherBreak => this["NeedAnotherBreak"];
    public string MiniGames => this["MiniGames"];
    public string ProductivityOverrated => this["ProductivityOverrated"];
    public string ShuffleGames => this["ShuffleGames"];
    public string ExcuseGenerator => this["ExcuseGenerator"];
    public string NeedAReason => this["NeedAReason"];
    public string TapForExcuse => this["TapForExcuse"];
    public string GenerateExcuse => this["GenerateExcuse"];
    public string CopyToClipboard => this["CopyToClipboard"];
    public string YourStats => this["YourStats"];
    public string BeProud => this["BeProud"];
    public string TasksAvoided => this["TasksAvoided"];
    public string BreaksTaken => this["BreaksTaken"];
    public string ExcusesGeneratedStat => this["ExcusesGeneratedStat"];
    public string GamesPlayed => this["GamesPlayed"];
    public string AchievementUnlocked => this["AchievementUnlocked"];
}
