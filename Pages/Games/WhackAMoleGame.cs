using procrastinate.Resources.Strings;

namespace procrastinate.Pages.Games;

public class WhackAMoleGame : MiniGame
{
    public override string Name => AppStrings.GetString("WhackAMole");
    public override string Icon => "\uf6d3";
    public override string IconColor => "#A855F7";
    public override string Description => AppStrings.GetString("WhackAMoleDesc");

    private const int GridSize = 3;
    private readonly Button[,] _holes = new Button[GridSize, GridSize];
    private int _score;
    private int _misses;
    private bool _running;
    private CancellationTokenSource? _cts;
    private Label? _scoreLabel;
    private Button? _startBtn;

    public override View CreateGameView()
    {
        _scoreLabel = new Label { Text = AppStrings.GetString("ScoreAndMisses", 0, 0), HorizontalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromArgb("#CBD5E1") };
        _startBtn = new Button { Text = AppStrings.GetString("StartGame30s"), BackgroundColor = Color.FromArgb("#A855F7"), TextColor = Colors.White, CornerRadius = 12, HeightRequest = 44 };
        _startBtn.Clicked += OnStartClicked;

        var grid = new Grid { ColumnSpacing = 8, RowSpacing = 8, HorizontalOptions = LayoutOptions.Center };
        for (int i = 0; i < GridSize; i++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition(80));
            grid.RowDefinitions.Add(new RowDefinition(80));
        }

        for (int r = 0; r < GridSize; r++)
        {
            for (int c = 0; c < GridSize; c++)
            {
                var row = r;
                var col = c;
                _holes[r, c] = new Button
                {
                    BackgroundColor = Color.FromArgb("#334155"),
                    FontSize = 36,
                    CornerRadius = 40,
                    Text = "🕳️"
                };
                _holes[r, c].Clicked += (s, e) => OnHoleClicked(row, col);
                grid.Add(_holes[r, c], c, r);
            }
        }

        return new VerticalStackLayout
        {
            Spacing = 16,
            Children = { grid, _scoreLabel, _startBtn }
        };
    }

    private async void OnStartClicked(object? sender, EventArgs e)
    {
        if (_running) return;
        OnGamePlayed?.Invoke();

        _score = 0;
        _misses = 0;
        _running = true;
        _cts = new CancellationTokenSource();
        _startBtn!.IsEnabled = false;
        UpdateScore();

        // Reset all holes
        for (int r = 0; r < GridSize; r++)
            for (int c = 0; c < GridSize; c++)
                _holes[r, c].Text = "🕳️";

        var endTime = DateTime.Now.AddSeconds(30);

        try
        {
            while (DateTime.Now < endTime && !_cts.Token.IsCancellationRequested)
            {
                // Show a mole
                int moleR = Random.Shared.Next(GridSize);
                int moleC = Random.Shared.Next(GridSize);
                _holes[moleR, moleC].Text = "🐹";

                // Wait for tap or timeout
                var showTime = Random.Shared.Next(600, 1200);
                await Task.Delay(showTime, _cts.Token);

                // If still showing, it's a miss
                if (_holes[moleR, moleC].Text == "🐹")
                {
                    _holes[moleR, moleC].Text = "🕳️";
                    _misses++;
                    UpdateScore();
                }

                await Task.Delay(200, _cts.Token);
            }
        }
        catch (TaskCanceledException) { }

        _running = false;
        _startBtn.IsEnabled = true;
        _scoreLabel!.Text = AppStrings.GetString("GameOverScore", _score);
    }

    private void OnHoleClicked(int row, int col)
    {
        if (!_running) return;

        if (_holes[row, col].Text == "🐹")
        {
            _holes[row, col].Text = "💥";
            _score++;
            UpdateScore();
            Task.Delay(100).ContinueWith(_ =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (_holes[row, col].Text == "💥")
                        _holes[row, col].Text = "🕳️";
                });
            });
        }
    }

    private void UpdateScore()
    {
        _scoreLabel!.Text = AppStrings.GetString("ScoreAndMisses", _score, _misses);
    }

    public override void StartGame() { }
    public override void StopGame()
    {
        _cts?.Cancel();
        _running = false;
    }
}
