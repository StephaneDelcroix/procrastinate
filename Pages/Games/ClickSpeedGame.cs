namespace procrastinate.Pages.Games;

public class ClickSpeedGame : MiniGame
{
    public override string Name => "Click Speed";
    public override string Icon => "\uf0e7";
    public override string IconColor => "#F59E0B";
    public override string Description => "How many clicks in 5 seconds?";

    private int _clickCount;
    private bool _active;
    private Button? _clickBtn, _startBtn;
    private Label? _scoreLabel;

    public override View CreateGameView()
    {
        _scoreLabel = new Label { Text = "Clicks: 0", HorizontalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromArgb("#CBD5E1") };
        _clickBtn = new Button { Text = "Click Me!", HeightRequest = 80, FontSize = 22, BackgroundColor = Color.FromArgb("#F59E0B"), TextColor = Color.FromArgb("#0F172A"), CornerRadius = 16, IsEnabled = false };
        _startBtn = new Button { Text = "Start Challenge", BackgroundColor = Color.FromArgb("#334155"), TextColor = Color.FromArgb("#E2E8F0"), CornerRadius = 12, HeightRequest = 44 };

        _clickBtn.Clicked += OnClickBtnClicked;
        _startBtn.Clicked += OnStartClicked;

        return new VerticalStackLayout
        {
            Spacing = 16,
            Children = { _clickBtn, _scoreLabel, _startBtn }
        };
    }

    private async void OnStartClicked(object? sender, EventArgs e)
    {
        OnGamePlayed?.Invoke();
        _clickCount = 0;
        _scoreLabel!.Text = "Clicks: 0";
        _startBtn!.IsEnabled = false;
        _clickBtn!.IsEnabled = true;
        _active = true;

        await Task.Delay(5000);

        _active = false;
        _clickBtn.IsEnabled = false;
        _startBtn.IsEnabled = true;
        _scoreLabel.Text = $"Final: {_clickCount} clicks! ({_clickCount / 5.0:F1}/sec)";
    }

    private void OnClickBtnClicked(object? sender, EventArgs e)
    {
        if (!_active) return;
        _clickCount++;
        _scoreLabel!.Text = $"Clicks: {_clickCount}";
    }

    public override void StartGame() { }
    public override void StopGame() { _active = false; }
}
