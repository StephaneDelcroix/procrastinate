using procrastinate.Services;

namespace procrastinate.Pages;

public partial class TasksPage : ContentPage
{
    private readonly StatsService _statsService;
    private readonly string[] _excuses = [
        "Oops! The task list is full. Try again tomorrow!",
        "Error 404: Productivity not found.",
        "Adding tasks requires premium subscription ($999/month).",
        "Task rejected: You deserve a break instead!",
        "Server is napping. Just like you should be!",
        "Maximum productivity reached! (1 task = maximum)",
        "Your task was eaten by a virtual dog. Sorry!",
        "New tasks are on backorder until next year."
    ];

    public TasksPage(StatsService statsService)
    {
        InitializeComponent();
        _statsService = statsService;
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }

    private async void OnTaskChecked(object? sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            _statsService.IncrementBreaksTaken();
            MotivationLabel.Text = "🎉 Congratulations! You completed ALL your tasks!";
            await Task.Delay(2000);
            TaskCheckBox.IsChecked = false;
            MotivationLabel.Text = "Wait... you still need another break!";
        }
    }

    private async void OnAddTaskClicked(object? sender, EventArgs e)
    {
        _statsService.IncrementTasksAvoided();
        var excuse = _excuses[Random.Shared.Next(_excuses.Length)];
        await DisplayAlertAsync("Cannot Add Task", excuse, "OK, I'll rest instead");
    }
}
