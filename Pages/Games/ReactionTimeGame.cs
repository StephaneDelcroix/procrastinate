namespace procrastinate.Pages.Games;

public class ReactionTimeGame : MiniGame
{
    public override string Name => "Reaction Time";
    public override string Icon => "\uf192";
    public override string IconColor => "#EF4444";
    public override string Description => "Wait for green, then tap!";

    private DateTime _startTime;
    private bool _waiting, _ready;
    private CancellationTokenSource? _cts;
    private Button? _reactionBtn, _startBtn;
    private Label? _resultLabel;

    public override View CreateGameView()
    {
        _resultLabel = new Label { Text = "Tap 'Start' to begin", HorizontalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromArgb("#CBD5E1") };
        _reactionBtn = new Button { Text = "Wait...", HeightRequest = 80, FontSize = 22, BackgroundColor = Color.FromArgb("#EF4444"), TextColor = Colors.White, CornerRadius = 16, IsEnabled = false };
        _startBtn = new Button { Text = "Start", BackgroundColor = Color.FromArgb("#334155"), TextColor = Color.FromArgb("#E2E8F0"), CornerRadius = 12, HeightRequest = 44 };

        _reactionBtn.Clicked += OnReactionClicked;
        _startBtn.Clicked += OnStartClicked;

        return new VerticalStackLayout
        {
            Spacing = 16,
            Children = { _reactionBtn, _resultLabel, _startBtn }
        };
    }

    private async void OnStartClicked(object? sender, EventArgs e)
    {
        OnGamePlayed?.Invoke();
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        _startBtn!.IsEnabled = false;
        _reactionBtn!.IsEnabled = true;
        _reactionBtn.BackgroundColor = Colors.Red;
        _reactionBtn.Text = "Wait...";
        _resultLabel!.Text = "Wait for green!";
        _waiting = true;
        _ready = false;

        var delay = Random.Shared.Next(2000, 5000);
        try
        {
            await Task.Delay(delay, _cts.Token);
            _waiting = false;
            _ready = true;
            _startTime = DateTime.Now;
            _reactionBtn.BackgroundColor = Colors.Green;
            _reactionBtn.Text = "TAP NOW!";
        }
        catch (TaskCanceledException) { }
    }

    private void OnReactionClicked(object? sender, EventArgs e)
    {
        if (_waiting)
        {
            _cts?.Cancel();
            _resultLabel!.Text = "Too early! Try again.";
            _reactionBtn!.BackgroundColor = Colors.Orange;
            _reactionBtn.Text = "Too soon!";
            _startBtn!.IsEnabled = true;
            _reactionBtn.IsEnabled = false;
        }
        else if (_ready)
        {
            var time = (DateTime.Now - _startTime).TotalMilliseconds;
            _resultLabel!.Text = $"Reaction time: {time:F0}ms";
            _reactionBtn!.BackgroundColor = Colors.Gray;
            _reactionBtn.Text = "Done!";
            _startBtn!.IsEnabled = true;
            _reactionBtn.IsEnabled = false;
            _ready = false;
        }
    }

    public override void StartGame() { }
    public override void StopGame() { _cts?.Cancel(); _waiting = false; _ready = false; }
}
