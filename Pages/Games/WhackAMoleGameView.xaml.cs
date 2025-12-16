using procrastinate.Resources.Strings;

namespace procrastinate.Pages.Games;

public partial class WhackAMoleGameView : ContentView
{
    private const int GridSize = 3;
    private readonly Button[,] _holes = new Button[GridSize, GridSize];
    private int _score;
    private int _misses;
    private bool _running;
    private CancellationTokenSource? _cts;
    
    public Action? OnGamePlayed { get; set; }

    public WhackAMoleGameView()
    {
        InitializeComponent();
        CreateHoles();
    }

    private void CreateHoles()
    {
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
                    Text = "O"
                };
                _holes[r, c].Clicked += (s, e) => OnHoleClicked(row, col);
                GameGrid.Add(_holes[r, c], c, r);
            }
        }
    }

    private async void OnStartClicked(object? sender, EventArgs e)
    {
        if (_running) return;
        OnGamePlayed?.Invoke();

        _score = 0;
        _misses = 0;
        _running = true;
        _cts = new CancellationTokenSource();
        StartBtn.IsEnabled = false;
        UpdateScore();

        for (int r = 0; r < GridSize; r++)
            for (int c = 0; c < GridSize; c++)
                _holes[r, c].Text = "O";

        var endTime = DateTime.Now.AddSeconds(30);

        try
        {
            while (DateTime.Now < endTime && !_cts.Token.IsCancellationRequested)
            {
                int moleR = Random.Shared.Next(GridSize);
                int moleC = Random.Shared.Next(GridSize);
                _holes[moleR, moleC].Text = "X";

                var showTime = Random.Shared.Next(600, 1200);
                await Task.Delay(showTime, _cts.Token);

                if (_holes[moleR, moleC].Text == "X")
                {
                    _holes[moleR, moleC].Text = "O";
                    _misses++;
                    UpdateScore();
                }

                await Task.Delay(200, _cts.Token);
            }
        }
        catch (TaskCanceledException) { }

        _running = false;
        StartBtn.IsEnabled = true;
        ScoreLabel.Text = AppStrings.GetString("GameOverScore", _score);
    }

    private void OnHoleClicked(int row, int col)
    {
        if (!_running) return;

        if (_holes[row, col].Text == "X")
        {
            _holes[row, col].Text = "!";
            _score++;
            UpdateScore();
            Task.Delay(100).ContinueWith(_ =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (_holes[row, col].Text == "!")
                        _holes[row, col].Text = "O";
                });
            });
        }
    }

    private void UpdateScore()
    {
        ScoreLabel.Text = AppStrings.GetString("ScoreAndMisses", _score, _misses);
    }

    public void Stop()
    {
        _cts?.Cancel();
        _running = false;
    }
}
