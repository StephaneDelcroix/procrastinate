using procrastinate.Resources.Strings;

namespace procrastinate.Pages.Games;

public class MinesweeperGame : MiniGame
{
    public override string Name => AppStrings.GetString("Minesweeper");
    public override string Icon => "\uf1e2";
    public override string IconColor => "#22C55E";
    public override string Description => AppStrings.GetString("MinesweeperDesc");

    private const int Size = 6;
    private const int Mines = 5;
    private readonly Button[,] _cells = new Button[Size, Size];
    private readonly bool[,] _isMine = new bool[Size, Size];
    private readonly bool[,] _revealed = new bool[Size, Size];
    private bool _gameOver;
    private bool _firstClick = true;
    private Label? _statusLabel;
    private Button? _resetBtn;

    public override View CreateGameView()
    {
        _statusLabel = new Label { Text = AppStrings.GetString("FindSafeCells", Mines), HorizontalOptions = LayoutOptions.Center, FontSize = 14, TextColor = Color.FromArgb("#CBD5E1") };
        _resetBtn = new Button { Text = AppStrings.GetString("NewGame"), BackgroundColor = Color.FromArgb("#22C55E"), TextColor = Color.FromArgb("#0F172A"), CornerRadius = 12, HeightRequest = 44 };
        _resetBtn.Clicked += (s, e) => ResetGame();

        var grid = new Grid { ColumnSpacing = 2, RowSpacing = 2, HorizontalOptions = LayoutOptions.Center };
        for (int i = 0; i < Size; i++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition(45));
            grid.RowDefinitions.Add(new RowDefinition(45));
        }

        for (int r = 0; r < Size; r++)
        {
            for (int c = 0; c < Size; c++)
            {
                var row = r;
                var col = c;
                _cells[r, c] = new Button
                {
                    BackgroundColor = Color.FromArgb("#334155"),
                    TextColor = Colors.White,
                    FontSize = 16,
                    CornerRadius = 6,
                    Padding = 0
                };
                _cells[r, c].Clicked += (s, e) => OnCellClicked(row, col);
                grid.Add(_cells[r, c], c, r);
            }
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
        _gameOver = false;
        _firstClick = true;

        for (int r = 0; r < Size; r++)
        {
            for (int c = 0; c < Size; c++)
            {
                _isMine[r, c] = false;
                _revealed[r, c] = false;
                _cells[r, c].Text = "";
                _cells[r, c].BackgroundColor = Color.FromArgb("#334155");
                _cells[r, c].IsEnabled = true;
            }
        }

        _statusLabel!.Text = AppStrings.GetString("FindSafeCells", Mines);
    }

    private void PlaceMines(int safeRow, int safeCol)
    {
        int placed = 0;
        while (placed < Mines)
        {
            int r = Random.Shared.Next(Size);
            int c = Random.Shared.Next(Size);
            if (!_isMine[r, c] && !(r == safeRow && c == safeCol))
            {
                _isMine[r, c] = true;
                placed++;
            }
        }
    }

    private void OnCellClicked(int row, int col)
    {
        if (_gameOver || _revealed[row, col]) return;

        if (_firstClick)
        {
            PlaceMines(row, col);
            _firstClick = false;
        }

        RevealCell(row, col);
        CheckWin();
    }

    private void RevealCell(int row, int col)
    {
        if (row < 0 || row >= Size || col < 0 || col >= Size) return;
        if (_revealed[row, col]) return;

        _revealed[row, col] = true;

        if (_isMine[row, col])
        {
            _cells[row, col].Text = "💥";
            _cells[row, col].BackgroundColor = Color.FromArgb("#EF4444");
            GameOver(false);
            return;
        }

        int count = CountAdjacentMines(row, col);
        _cells[row, col].BackgroundColor = Color.FromArgb("#1E293B");

        if (count > 0)
        {
            _cells[row, col].Text = count.ToString();
            _cells[row, col].TextColor = count switch
            {
                1 => Color.FromArgb("#3B82F6"),
                2 => Color.FromArgb("#22C55E"),
                3 => Color.FromArgb("#EF4444"),
                _ => Color.FromArgb("#F59E0B")
            };
        }
        else
        {
            // Reveal adjacent cells
            for (int dr = -1; dr <= 1; dr++)
                for (int dc = -1; dc <= 1; dc++)
                    if (dr != 0 || dc != 0)
                        RevealCell(row + dr, col + dc);
        }
    }

    private int CountAdjacentMines(int row, int col)
    {
        int count = 0;
        for (int dr = -1; dr <= 1; dr++)
            for (int dc = -1; dc <= 1; dc++)
            {
                int r = row + dr, c = col + dc;
                if (r >= 0 && r < Size && c >= 0 && c < Size && _isMine[r, c])
                    count++;
            }
        return count;
    }

    private void CheckWin()
    {
        int unrevealed = 0;
        for (int r = 0; r < Size; r++)
            for (int c = 0; c < Size; c++)
                if (!_revealed[r, c])
                    unrevealed++;

        if (unrevealed == Mines)
            GameOver(true);
    }

    private void GameOver(bool won)
    {
        _gameOver = true;
        _statusLabel!.Text = won ? AppStrings.GetString("YouWin") : AppStrings.GetString("BoomGameOver");

        // Reveal all mines
        for (int r = 0; r < Size; r++)
            for (int c = 0; c < Size; c++)
                if (_isMine[r, c] && !_revealed[r, c])
                {
                    _cells[r, c].Text = "💣";
                    _cells[r, c].BackgroundColor = Color.FromArgb("#7F1D1D");
                }
    }

    public override void StartGame() => ResetGame();
    public override void StopGame() => _gameOver = true;
}
