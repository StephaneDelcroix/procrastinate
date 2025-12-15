using procrastinate.Services;

namespace procrastinate.Pages;

public partial class StatsPage : ContentPage
{
    private readonly StatsService _statsService;

    private readonly string[] _achievements = [
        "Professional Break Taker 🛋️",
        "Master of Tomorrow 📅",
        "Expert Excuse Artist 🎨",
        "Productivity Rebel 😎",
        "Time Well Wasted ⏰",
        "Champion Procrastinator 🏆",
        "Couch Potato Elite 🥔",
        "Task Avoidance Guru 🧘"
    ];

    public StatsPage(StatsService statsService)
    {
        InitializeComponent();
        _statsService = statsService;
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

        var totalActivity = _statsService.TasksAvoided + _statsService.BreaksTaken + 
                           _statsService.ExcusesGenerated + _statsService.GamesPlayed;
        
        AchievementLabel.Text = totalActivity switch
        {
            0 => "Getting Started: Open the app! ✅",
            < 5 => "Beginner Procrastinator 🐣",
            < 15 => _achievements[Random.Shared.Next(_achievements.Length)],
            _ => "🌟 LEGENDARY PROCRASTINATOR 🌟"
        };
    }
}
