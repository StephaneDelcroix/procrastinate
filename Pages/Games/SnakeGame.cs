namespace procrastinate.Pages.Games;

public class SnakeGame : MiniGame
{
    public override string Name => "Tilt Snake";
    public override string Icon => "\uf7a0";
    public override string IconColor => "#14B8A6";
    public override string Description => "Tilt your phone to guide the snake!";

    private const int GridSize = 10;
    private const int CellSize = 28;
    private readonly List<(int r, int c)> _snake = [];
    private (int r, int c) _food;
    private (int dr, int dc) _direction = (0, 1);
    private bool _running;
    private int _score;
    private Label? _scoreLabel, _statusLabel;
    private Button? _startBtn;
    private Grid? _gameGrid;
    private readonly BoxView[,] _cells = new BoxView[GridSize, GridSize];

    public override View CreateGameView()
    {
        _scoreLabel = new Label { Text = "Score: 0", HorizontalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromArgb("#CBD5E1") };
        _statusLabel = new Label { Text = "Tilt phone to move!", HorizontalOptions = LayoutOptions.Center, FontSize = 12, TextColor = Color.FromArgb("#64748B") };
        _startBtn = new Button { Text = "Start Game", BackgroundColor = Color.FromArgb("#14B8A6"), TextColor = Color.FromArgb("#0F172A"), CornerRadius = 12, HeightRequest = 44 };
        _startBtn.Clicked += OnStartClicked;

        _gameGrid = new Grid { ColumnSpacing = 1, RowSpacing = 1, HorizontalOptions = LayoutOptions.Center, BackgroundColor = Color.FromArgb("#1E293B") };
        for (int i = 0; i < GridSize; i++)
        {
            _gameGrid.ColumnDefinitions.Add(new ColumnDefinition(CellSize));
            _gameGrid.RowDefinitions.Add(new RowDefinition(CellSize));
        }

        for (int r = 0; r < GridSize; r++)
        {
            for (int c = 0; c < GridSize; c++)
            {
                _cells[r, c] = new BoxView { BackgroundColor = Color.FromArgb("#0F172A"), CornerRadius = 4 };
                _gameGrid.Add(_cells[r, c], c, r);
            }
        }

        return new VerticalStackLayout
        {
            Spacing = 12,
            Children = { _gameGrid, _scoreLabel, _statusLabel, _startBtn }
        };
    }

    private async void OnStartClicked(object? sender, EventArgs e)
    {
        if (_running) return;
        OnGamePlayed?.Invoke();

        _snake.Clear();
        _snake.Add((GridSize / 2, GridSize / 2));
        _direction = (0, 1);
        _score = 0;
        _scoreLabel!.Text = "Score: 0";
        _running = true;
        _startBtn!.IsEnabled = false;

        SpawnFood();
        Accelerometer.ReadingChanged += OnAccelerometerReading;

        try { Accelerometer.Start(SensorSpeed.Game); }
        catch { _statusLabel!.Text = "Accelerometer not available"; }

        await GameLoop();
    }

    private void OnAccelerometerReading(object? sender, AccelerometerChangedEventArgs e)
    {
        var data = e.Reading;
        // Tilt controls: X for left/right, Y for up/down
        if (Math.Abs(data.Acceleration.X) > Math.Abs(data.Acceleration.Y))
        {
            _direction = data.Acceleration.X > 0.3 ? (0, -1) : data.Acceleration.X < -0.3 ? (0, 1) : _direction;
        }
        else
        {
            _direction = data.Acceleration.Y > 0.3 ? (1, 0) : data.Acceleration.Y < -0.3 ? (-1, 0) : _direction;
        }
    }

    private async Task GameLoop()
    {
        while (_running)
        {
            await Task.Delay(200);
            if (!_running) break;

            var head = _snake[0];
            var newHead = (r: head.r + _direction.dr, c: head.c + _direction.dc);

            // Check collision with walls
            if (newHead.r < 0 || newHead.r >= GridSize || newHead.c < 0 || newHead.c >= GridSize)
            {
                GameOver();
                return;
            }

            // Check collision with self
            if (_snake.Contains(newHead))
            {
                GameOver();
                return;
            }

            _snake.Insert(0, newHead);

            if (newHead == _food)
            {
                _score++;
                _scoreLabel!.Text = $"Score: {_score}";
                SpawnFood();
            }
            else
            {
                _snake.RemoveAt(_snake.Count - 1);
            }

            UpdateGrid();
        }
    }

    private void SpawnFood()
    {
        var empty = new List<(int r, int c)>();
        for (int r = 0; r < GridSize; r++)
            for (int c = 0; c < GridSize; c++)
                if (!_snake.Contains((r, c)))
                    empty.Add((r, c));

        if (empty.Count > 0)
            _food = empty[Random.Shared.Next(empty.Count)];
    }

    private void UpdateGrid()
    {
        for (int r = 0; r < GridSize; r++)
        {
            for (int c = 0; c < GridSize; c++)
            {
                if (_snake.Contains((r, c)))
                    _cells[r, c].BackgroundColor = _snake[0] == (r, c) ? Color.FromArgb("#14B8A6") : Color.FromArgb("#5EEAD4");
                else if (_food == (r, c))
                    _cells[r, c].BackgroundColor = Color.FromArgb("#EF4444");
                else
                    _cells[r, c].BackgroundColor = Color.FromArgb("#0F172A");
            }
        }
    }

    private void GameOver()
    {
        _running = false;
        try { Accelerometer.Stop(); } catch { }
        Accelerometer.ReadingChanged -= OnAccelerometerReading;
        _statusLabel!.Text = $"Game Over! Score: {_score}";
        _startBtn!.IsEnabled = true;
    }

    public override void StartGame() { }
    public override void StopGame()
    {
        _running = false;
        try { Accelerometer.Stop(); } catch { }
        Accelerometer.ReadingChanged -= OnAccelerometerReading;
    }
}
