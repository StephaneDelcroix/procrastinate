using procrastinate.Resources.Strings;

namespace procrastinate.Pages.Games;

public class NumberGuessingGame : MiniGame
{
    public override string Name => AppStrings.GetString("NumberGuess");
    public override string Icon => "\uf059";
    public override string IconColor => "#F59E0B";
    public override string Description => AppStrings.GetString("NumberGuessDesc");

    private int _target;
    private int _attempts;
    private bool _gameOver;
    private Entry? _guessEntry;
    private Label? _resultLabel, _attemptsLabel;
    private Button? _guessBtn, _newGameBtn;

    public override View CreateGameView()
    {
        _resultLabel = new Label { Text = AppStrings.GetString("ThinkingOfNumber"), HorizontalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromArgb("#CBD5E1") };
        _attemptsLabel = new Label { Text = AppStrings.GetString("Attempts", 0), HorizontalOptions = LayoutOptions.Center, FontSize = 14, TextColor = Color.FromArgb("#64748B") };
        
        _guessEntry = new Entry
        {
            Placeholder = AppStrings.GetString("EnterGuess"),
            Keyboard = Keyboard.Numeric,
            HorizontalOptions = LayoutOptions.Fill,
            BackgroundColor = Color.FromArgb("#334155"),
            TextColor = Colors.White,
            PlaceholderColor = Color.FromArgb("#64748B")
        };

        _guessBtn = new Button { Text = AppStrings.GetString("Guess"), BackgroundColor = Color.FromArgb("#F59E0B"), TextColor = Color.FromArgb("#0F172A"), CornerRadius = 12, HeightRequest = 44 };
        _guessBtn.Clicked += OnGuessClicked;

        _newGameBtn = new Button { Text = AppStrings.GetString("NewGame"), BackgroundColor = Color.FromArgb("#334155"), TextColor = Color.FromArgb("#E2E8F0"), CornerRadius = 12, HeightRequest = 44 };
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
        _resultLabel!.Text = AppStrings.GetString("ThinkingOfNumber");
        _attemptsLabel!.Text = AppStrings.GetString("Attempts", 0);
        _guessEntry!.Text = "";
        _guessEntry.IsEnabled = true;
        _guessBtn!.IsEnabled = true;
    }

    private void OnGuessClicked(object? sender, EventArgs e)
    {
        if (_gameOver) return;
        if (!int.TryParse(_guessEntry!.Text, out int guess) || guess < 1 || guess > 100)
        {
            _resultLabel!.Text = AppStrings.GetString("EnterNumber1to100");
            return;
        }

        _attempts++;
        _attemptsLabel!.Text = AppStrings.GetString("Attempts", _attempts);

        if (guess == _target)
        {
            _resultLabel!.Text = AppStrings.GetString("Correct", _target);
            _resultLabel.TextColor = Color.FromArgb("#22C55E");
            _gameOver = true;
            _guessEntry.IsEnabled = false;
            _guessBtn!.IsEnabled = false;
        }
        else if (guess < _target)
        {
            _resultLabel!.Text = AppStrings.GetString("TooLow", guess);
            _resultLabel.TextColor = Color.FromArgb("#3B82F6");
        }
        else
        {
            _resultLabel!.Text = AppStrings.GetString("TooHigh", guess);
            _resultLabel.TextColor = Color.FromArgb("#EF4444");
        }

        _guessEntry.Text = "";
    }

    public override void StartGame() => ResetGame();
    public override void StopGame() => _gameOver = true;
}
