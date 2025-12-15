namespace procrastinate.Pages.Games;

public class NumberGuessingGame : MiniGame
{
    public override string Name => "Number Guess";
    public override string Icon => "\uf059";
    public override string IconColor => "#F59E0B";
    public override string Description => "Guess the number 1-100!";

    private int _target;
    private int _attempts;
    private bool _gameOver;
    private Entry? _guessEntry;
    private Label? _resultLabel, _attemptsLabel;
    private Button? _guessBtn, _newGameBtn;

    public override View CreateGameView()
    {
        _resultLabel = new Label { Text = "I'm thinking of a number 1-100...", HorizontalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromArgb("#CBD5E1") };
        _attemptsLabel = new Label { Text = "Attempts: 0", HorizontalOptions = LayoutOptions.Center, FontSize = 14, TextColor = Color.FromArgb("#64748B") };
        
        _guessEntry = new Entry
        {
            Placeholder = "Enter your guess",
            Keyboard = Keyboard.Numeric,
            HorizontalOptions = LayoutOptions.Fill,
            BackgroundColor = Color.FromArgb("#334155"),
            TextColor = Colors.White,
            PlaceholderColor = Color.FromArgb("#64748B")
        };

        _guessBtn = new Button { Text = "Guess!", BackgroundColor = Color.FromArgb("#F59E0B"), TextColor = Color.FromArgb("#0F172A"), CornerRadius = 12, HeightRequest = 44 };
        _guessBtn.Clicked += OnGuessClicked;

        _newGameBtn = new Button { Text = "New Game", BackgroundColor = Color.FromArgb("#334155"), TextColor = Color.FromArgb("#E2E8F0"), CornerRadius = 12, HeightRequest = 44 };
        _newGameBtn.Clicked += (s, e) => ResetGame();

        ResetGame();

        return new VerticalStackLayout
        {
            Spacing = 12,
            Children = { _resultLabel, _guessEntry, _guessBtn, _attemptsLabel, _newGameBtn }
        };
    }

    private void ResetGame()
    {
        OnGamePlayed?.Invoke();
        _target = Random.Shared.Next(1, 101);
        _attempts = 0;
        _gameOver = false;
        _resultLabel!.Text = "I'm thinking of a number 1-100...";
        _attemptsLabel!.Text = "Attempts: 0";
        _guessEntry!.Text = "";
        _guessEntry.IsEnabled = true;
        _guessBtn!.IsEnabled = true;
    }

    private void OnGuessClicked(object? sender, EventArgs e)
    {
        if (_gameOver) return;
        if (!int.TryParse(_guessEntry!.Text, out int guess) || guess < 1 || guess > 100)
        {
            _resultLabel!.Text = "Please enter a number 1-100";
            return;
        }

        _attempts++;
        _attemptsLabel!.Text = $"Attempts: {_attempts}";

        if (guess == _target)
        {
            _resultLabel!.Text = $"🎉 Correct! It was {_target}!";
            _resultLabel.TextColor = Color.FromArgb("#22C55E");
            _gameOver = true;
            _guessEntry.IsEnabled = false;
            _guessBtn!.IsEnabled = false;
        }
        else if (guess < _target)
        {
            _resultLabel!.Text = $"📈 {guess} is too LOW!";
            _resultLabel.TextColor = Color.FromArgb("#3B82F6");
        }
        else
        {
            _resultLabel!.Text = $"📉 {guess} is too HIGH!";
            _resultLabel.TextColor = Color.FromArgb("#EF4444");
        }

        _guessEntry.Text = "";
    }

    public override void StartGame() => ResetGame();
    public override void StopGame() => _gameOver = true;
}
