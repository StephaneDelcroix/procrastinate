using procrastinate.Resources.Strings;

namespace procrastinate.Pages.Games;

public partial class ClickSpeedGameView : ContentView
{
    private int _clickCount;
    private bool _active;
    
    public Action? OnGamePlayed { get; set; }

    public ClickSpeedGameView()
    {
        InitializeComponent();
        UpdateScoreLabel();
    }

    private void UpdateScoreLabel()
    {
        ScoreLabel.Text = AppStrings.GetString("Clicks", _clickCount);
    }

    private async void OnStartClicked(object? sender, EventArgs e)
    {
        OnGamePlayed?.Invoke();
        _clickCount = 0;
        UpdateScoreLabel();
        StartBtn.IsEnabled = false;
        ClickBtn.IsEnabled = true;
        _active = true;

        await Task.Delay(5000);

        _active = false;
        ClickBtn.IsEnabled = false;
        StartBtn.IsEnabled = true;
        ScoreLabel.Text = AppStrings.GetString("FinalClicks", _clickCount, _clickCount / 5.0);
    }

    private void OnClickBtnClicked(object? sender, EventArgs e)
    {
        if (!_active) return;
        _clickCount++;
        UpdateScoreLabel();
    }

    public void Stop()
    {
        _active = false;
    }
}
