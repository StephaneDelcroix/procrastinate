using procrastinate.Resources.Strings;

namespace procrastinate.Pages.Games;

public class MemoryMatchGame : MiniGame
{
    public override string Name => AppStrings.GetString("MemoryMatch");
    public override string Icon => "\uf5fd";
    public override string IconColor => "#EC4899";
    public override string Description => AppStrings.GetString("MemoryMatchDesc");

    private readonly string[] _symbols = ["🍎", "🍊", "🍋", "🍇", "🍓", "🍒", "🥝", "🍑"];
    private readonly Button[] _cards = new Button[16];
    private readonly string[] _cardValues = new string[16];
    private readonly bool[] _matched = new bool[16];
    private int _firstCard = -1;
    private int _secondCard = -1;
    private bool _checking;
    private int _moves;
    private int _pairsFound;
    private Label? _statusLabel;
    private Button? _resetBtn;

    public override View CreateGameView()
    {
        _statusLabel = new Label { Text = AppStrings.GetString("MovesAndPairs", 0, 0, 8), HorizontalOptions = LayoutOptions.Center, FontSize = 14, TextColor = Color.FromArgb("#CBD5E1") };
        _resetBtn = new Button { Text = AppStrings.GetString("NewGame"), BackgroundColor = Color.FromArgb("#EC4899"), TextColor = Colors.White, CornerRadius = 12, HeightRequest = 44 };
        _resetBtn.Clicked += (s, e) => ResetGame();

        var grid = new Grid
        {
            ColumnDefinitions = { new ColumnDefinition(60), new ColumnDefinition(60), new ColumnDefinition(60), new ColumnDefinition(60) },
            RowDefinitions = { new RowDefinition(60), new RowDefinition(60), new RowDefinition(60), new RowDefinition(60) },
            ColumnSpacing = 6,
            RowSpacing = 6,
            HorizontalOptions = LayoutOptions.Center
        };

        for (int i = 0; i < 16; i++)
        {
            var idx = i;
            _cards[i] = new Button
            {
                BackgroundColor = Color.FromArgb("#334155"),
                TextColor = Colors.White,
                FontSize = 24,
                CornerRadius = 8,
                Text = "?"
            };
            _cards[i].Clicked += (s, e) => OnCardClicked(idx);
            grid.Add(_cards[i], i % 4, i / 4);
        }

        ResetGame();

        return new VerticalStackLayout
        {
            Spacing = 12,
            Children = { grid, _statusLabel, _resetBtn }
        };
    }

    private void ResetGame()
    {
        OnGamePlayed?.Invoke();
        _moves = 0;
        _pairsFound = 0;
        _firstCard = -1;
        _secondCard = -1;
        _checking = false;

        // Create pairs and shuffle
        var values = new List<string>();
        foreach (var s in _symbols) { values.Add(s); values.Add(s); }
        
        for (int i = values.Count - 1; i > 0; i--)
        {
            int j = Random.Shared.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        for (int i = 0; i < 16; i++)
        {
            _cardValues[i] = values[i];
            _matched[i] = false;
            _cards[i].Text = "?";
            _cards[i].BackgroundColor = Color.FromArgb("#334155");
            _cards[i].IsEnabled = true;
        }

        UpdateStatus();
    }

    private async void OnCardClicked(int idx)
    {
        if (_checking || _matched[idx] || idx == _firstCard) return;

        _cards[idx].Text = _cardValues[idx];
        _cards[idx].BackgroundColor = Color.FromArgb("#1E293B");

        if (_firstCard == -1)
        {
            _firstCard = idx;
        }
        else
        {
            _secondCard = idx;
            _moves++;
            _checking = true;
            UpdateStatus();

            await Task.Delay(600);

            if (_cardValues[_firstCard] == _cardValues[_secondCard])
            {
                _matched[_firstCard] = true;
                _matched[_secondCard] = true;
                _cards[_firstCard].BackgroundColor = Color.FromArgb("#14B8A6");
                _cards[_secondCard].BackgroundColor = Color.FromArgb("#14B8A6");
                _pairsFound++;
                UpdateStatus();

                if (_pairsFound == 8)
                    _statusLabel!.Text = AppStrings.GetString("WinInMoves", _moves);
            }
            else
            {
                _cards[_firstCard].Text = "?";
                _cards[_secondCard].Text = "?";
                _cards[_firstCard].BackgroundColor = Color.FromArgb("#334155");
                _cards[_secondCard].BackgroundColor = Color.FromArgb("#334155");
            }

            _firstCard = -1;
            _secondCard = -1;
            _checking = false;
        }
    }

    private void UpdateStatus()
    {
        _statusLabel!.Text = AppStrings.GetString("MovesAndPairs", _moves, _pairsFound, 8);
    }

    public override void StartGame() => ResetGame();
    public override void StopGame() { _checking = false; }
}
