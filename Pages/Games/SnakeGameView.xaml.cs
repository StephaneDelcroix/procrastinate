using procrastinate.Resources.Strings;

namespace procrastinate.Pages.Games;

public partial class SnakeGameView : ContentView
{
    private const int GridSize = 10;
    private const int CellSize = 28;
    private readonly List<(int r, int c)> _snake = [];
    private (int r, int c) _food;
    private (int dr, int dc) _direction = (0, 1);
    private bool _running;
    private int _score;
    private readonly BoxView[,] _cells = new BoxView[GridSize, GridSize];
    
    public Action? OnGamePlayed { get; set; }

    public SnakeGameView()
    {
        InitializeComponent();
        CreateGrid();
    }

    private void CreateGrid()
    {
        for (int i = 0; i < GridSize; i++)
        {
            GameGrid.ColumnDefinitions.Add(new ColumnDefinition(CellSize));
            GameGrid.RowDefinitions.Add(new RowDefinition(CellSize));
        }

        for (int r = 0; r < GridSize; r++)
        {
            for (int c = 0; c < GridSize; c++)
            {
                _cells[r, c] = new BoxView { BackgroundColor = Color.FromArgb("#0F172A"), CornerRadius = 4 };
                GameGrid.Add(_cells[r, c], c, r);
            }
        }
    }

    private async void OnStartClicked(object? sender, EventArgs e)
    {
        if (_running) return;
        OnGamePlayed?.Invoke();

        _snake.Clear();
        _snake.Add((GridSize / 2, GridSize / 2));
        _direction = (0, 1);
        _score = 0;
        ScoreLabel.Text = AppStrings.GetString("Score", 0);
        _running = true;
        StartBtn.IsEnabled = false;

        SpawnFood();
        Accelerometer.ReadingChanged += OnAccelerometerReading;

        try { Accelerometer.Start(SensorSpeed.Game); }
        catch { StatusLabel.Text = AppStrings.GetString("AccelerometerNotAvailable"); }

        await GameLoop();
    }

    private void OnAccelerometerReading(object? sender, AccelerometerChangedEventArgs e)
    {
        var data = e.Reading;
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

            if (newHead.r < 0 || newHead.r >= GridSize || newHead.c < 0 || newHead.c >= GridSize)
            {
                GameOver();
                return;
            }

            if (_snake.Contains(newHead))
            {
                GameOver();
                return;
            }

            _snake.Insert(0, newHead);

            if (newHead == _food)
            {
                _score++;
                ScoreLabel.Text = AppStrings.GetString("Score", _score);
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
        StatusLabel.Text = AppStrings.GetString("GameOverScore", _score);
        StartBtn.IsEnabled = true;
    }

    public void Stop()
    {
        _running = false;
        try { Accelerometer.Stop(); } catch { }
        Accelerometer.ReadingChanged -= OnAccelerometerReading;
    }
}
