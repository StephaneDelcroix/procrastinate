using procrastinate.Resources.Strings;

namespace procrastinate.Pages.Games;

public class SimonSaysGame : MiniGame
{
    public override string Name => AppStrings.GetString("SimonSays");
    public override string Icon => "\uf111";
    public override string IconColor => "#EF4444";
    public override string Description => AppStrings.GetString("SimonSaysDesc");

    private readonly List<int> _sequence = [];
    private int _index;
    private bool _playerTurn;
    private int _score;
    private Button? _redBtn, _greenBtn, _blueBtn, _yellowBtn;
    private Label? _scoreLabel;
    private Button? _startBtn;

    public override View CreateGameView()
    {
        _scoreLabel = new Label { Text = AppStrings.GetString("Score", 0), HorizontalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromArgb("#CBD5E1") };
        _startBtn = new Button { Text = AppStrings.GetString("StartGame"), BackgroundColor = Color.FromArgb("#14B8A6"), TextColor = Color.FromArgb("#0F172A"), CornerRadius = 12, HeightRequest = 44 };
        _startBtn.Clicked += OnStartClicked;

        _redBtn = new Button { BackgroundColor = Color.FromArgb("#EF4444"), CornerRadius = 12, WidthRequest = 90, HeightRequest = 90 };
        _greenBtn = new Button { BackgroundColor = Color.FromArgb("#22C55E"), CornerRadius = 12, WidthRequest = 90, HeightRequest = 90 };
        _blueBtn = new Button { BackgroundColor = Color.FromArgb("#3B82F6"), CornerRadius = 12, WidthRequest = 90, HeightRequest = 90 };
        _yellowBtn = new Button { BackgroundColor = Color.FromArgb("#EAB308"), CornerRadius = 12, WidthRequest = 90, HeightRequest = 90 };

        _redBtn.Clicked += (s, e) => OnColorClicked(0);
        _greenBtn.Clicked += (s, e) => OnColorClicked(1);
        _blueBtn.Clicked += (s, e) => OnColorClicked(2);
        _yellowBtn.Clicked += (s, e) => OnColorClicked(3);

        var grid = new Grid
        {
            ColumnDefinitions = { new ColumnDefinition(new GridLength(95)), new ColumnDefinition(new GridLength(95)) },
            RowDefinitions = { new RowDefinition(new GridLength(95)), new RowDefinition(new GridLength(95)) },
            ColumnSpacing = 8,
            RowSpacing = 8,
            HorizontalOptions = LayoutOptions.Center
        };
        grid.Add(_redBtn, 0, 0);
        grid.Add(_greenBtn, 1, 0);
        grid.Add(_blueBtn, 0, 1);
        grid.Add(_yellowBtn, 1, 1);

        return new VerticalStackLayout
        {
            Spacing = 16,
            Children = { grid, _scoreLabel, _startBtn }
        };
    }

    private async void OnStartClicked(object? sender, EventArgs e)
    {
        OnGamePlayed?.Invoke();
        _sequence.Clear();
        _score = 0;
        _scoreLabel!.Text = AppStrings.GetString("Score", 0);
        _startBtn!.IsEnabled = false;
        await AddToSequence();
    }

    private async Task AddToSequence()
    {
        _sequence.Add(Random.Shared.Next(4));
        await PlaySequence();
    }

    private async Task PlaySequence()
    {
        _playerTurn = false;
        SetButtonsEnabled(false);
        await Task.Delay(500);

        foreach (var color in _sequence)
        {
            var btn = GetButton(color);
            var original = btn.BackgroundColor;
            btn.BackgroundColor = GetBrightColor(color);
            await Task.Delay(400);
            btn.BackgroundColor = original;
            await Task.Delay(200);
        }

        _playerTurn = true;
        _index = 0;
        SetButtonsEnabled(true);
    }

    private async void OnColorClicked(int color)
    {
        if (!_playerTurn) return;

        var btn = GetButton(color);
        var original = btn.BackgroundColor;
        btn.BackgroundColor = GetBrightColor(color);
        await Task.Delay(150);
        btn.BackgroundColor = original;

        if (color == _sequence[_index])
        {
            _index++;
            if (_index >= _sequence.Count)
            {
                _score++;
                _scoreLabel!.Text = AppStrings.GetString("Score", _score);
                await Task.Delay(500);
                _ = AddToSequence();
            }
        }
        else
        {
            _scoreLabel!.Text = AppStrings.GetString("GameOver", _score);
            _startBtn!.IsEnabled = true;
            SetButtonsEnabled(false);
        }
    }

    private Button GetButton(int i) => i switch { 0 => _redBtn!, 1 => _greenBtn!, 2 => _blueBtn!, _ => _yellowBtn! };
    private static Color GetBrightColor(int i) => i switch { 0 => Colors.Red, 1 => Colors.Lime, 2 => Colors.Blue, _ => Colors.Yellow };
    private void SetButtonsEnabled(bool e) { _redBtn!.IsEnabled = e; _greenBtn!.IsEnabled = e; _blueBtn!.IsEnabled = e; _yellowBtn!.IsEnabled = e; }

    public override void StartGame() { }
    public override void StopGame() { _playerTurn = false; }
}
