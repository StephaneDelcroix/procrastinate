using procrastinate.Resources.Strings;
using procrastinate.Services;

namespace procrastinate.Pages;

public partial class StatsPage : ContentPage
{
    private readonly StatsService _statsService;

    public StatsPage(StatsService statsService)
    {
        InitializeComponent();
        _statsService = statsService;
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshStats();
    }

    private void RefreshStats()
    {
        TasksAvoidedLabel.Text = _statsService.TasksAvoided.ToString();
        BreaksTakenLabel.Text = _statsService.BreaksTaken.ToString();
        ExcusesLabel.Text = _statsService.ExcusesGenerated.ToString();
        GamesPlayedLabel.Text = _statsService.GamesPlayed.ToString();
        AICallsLabel.Text = _statsService.AIExcuseCalls.ToString();

        RefreshHighScores();

        var totalActivity = _statsService.TasksAvoided + _statsService.BreaksTaken + 
                           _statsService.ExcusesGenerated + _statsService.GamesPlayed;
        
        AchievementLabel.Text = totalActivity switch
        {
            0 => $"{AppStrings.GetString("GettingStarted")} ✅",
            < 5 => $"{AppStrings.GetString("BeginnerProcrastinator")} 🐣",
            < 15 => GetRandomAchievement(),
            _ => $"🌟 {AppStrings.GetString("LegendaryProcrastinator")} 🌟"
        };
    }

    private void RefreshHighScores()
    {
        var highScores = _statsService.GameHighScores;
        
        if (highScores.Count == 0)
        {
            NoHighScoresLabel.IsVisible = true;
            return;
        }

        NoHighScoresLabel.IsVisible = false;
        
        // Clear existing scores (except the NoHighScoresLabel)
        var toRemove = HighScoresStack.Children.Where(c => c != NoHighScoresLabel).ToList();
        foreach (var child in toRemove)
            HighScoresStack.Children.Remove(child);

        foreach (var (game, score) in highScores.OrderByDescending(x => x.Value))
        {
            var scoreLabel = new Label
            {
                Text = $"{game}: {score}",
                FontSize = 16,
                TextColor = (Color)Application.Current!.Resources["Gray200"]
            };
            HighScoresStack.Children.Add(scoreLabel);
        }
    }

    private string GetRandomAchievement()
    {
        var achievements = AppStrings.CurrentLanguage switch
        {
            "fr" => new[] { "Professionnel de la pause 🛋️", "Maître de demain 📅", "Artiste des excuses 🎨", "Rebelle de la productivité 😎" },
            "es" => new[] { "Profesional del descanso 🛋️", "Maestro del mañana 📅", "Artista de excusas 🎨", "Rebelde de productividad 😎" },
            "pt" => new[] { "Profissional da pausa 🛋️", "Mestre do amanhã 📅", "Artista de desculpas 🎨", "Rebelde da produtividade 😎" },
            "nl" => new[] { "Professionele pauzenemer 🛋️", "Meester van morgen 📅", "Excuuskunstenaar 🎨", "Productiviteitsrebel 😎" },
            "cs" => new[] { "Profesionální pausař 🛋️", "Mistr zítřka 📅", "Umělec výmluv 🎨", "Rebel produktivity 😎" },
            _ => new[] { "Professional Break Taker 🛋️", "Master of Tomorrow 📅", "Expert Excuse Artist 🎨", "Productivity Rebel 😎" }
        };
        return achievements[Random.Shared.Next(achievements.Length)];
    }
}
