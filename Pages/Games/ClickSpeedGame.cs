using procrastinate.Resources.Strings;

namespace procrastinate.Pages.Games;

public class ClickSpeedGame : MiniGame
{
    public override string Name => AppStrings.GetString("ClickSpeed");
    public override string Icon => "\uf0e7";
    public override string IconColor => "#F59E0B";
    public override string Description => AppStrings.GetString("ClickSpeedDesc");

    private int _clickCount;
    private bool _active;
    private Button? _clickBtn, _startBtn;
    private Label? _scoreLabel;

    public override View CreateGameView()
    {
        _scoreLabel = new Label { Text = AppStrings.GetString("Clicks", 0), HorizontalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromArgb("#CBD5E1") };
        _clickBtn = new Button { Text = AppStrings.GetString("ClickMe"), HeightRequest = 80, FontSize = 22, BackgroundColor = Color.FromArgb("#F59E0B"), TextColor = Color.FromArgb("#0F172A"), CornerRadius = 16, IsEnabled = false };
        _startBtn = new Button { Text = AppStrings.GetString("StartChallenge"), BackgroundColor = Color.FromArgb("#334155"), TextColor = Color.FromArgb("#E2E8F0"), CornerRadius = 12, HeightRequest = 44 };

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
        _scoreLabel!.Text = AppStrings.GetString("Clicks", 0);
        _startBtn!.IsEnabled = false;
        _clickBtn!.IsEnabled = true;
        _active = true;

        await Task.Delay(5000);

        _active = false;
        _clickBtn.IsEnabled = false;
        _startBtn.IsEnabled = true;
        _scoreLabel.Text = AppStrings.GetString("FinalClicks", _clickCount, _clickCount / 5.0);
    }

    private void OnClickBtnClicked(object? sender, EventArgs e)
    {
        if (!_active) return;
        _clickCount++;
        _scoreLabel!.Text = AppStrings.GetString("Clicks", _clickCount);
    }

    public override void StartGame() { }
    public override void StopGame() { _active = false; }
}
