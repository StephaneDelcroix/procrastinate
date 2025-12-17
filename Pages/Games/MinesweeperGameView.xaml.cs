using procrastinate.Services;

namespace procrastinate.Pages.Games;

public partial class MinesweeperGameView : ContentView
{
    private const int Size = 6;
    private const int Mines = 5;
    private readonly Button[,] _cells = new Button[Size, Size];
    private readonly bool[,] _isMine = new bool[Size, Size];
    private readonly bool[,] _revealed = new bool[Size, Size];
    private bool _gameOver;
    private bool _firstClick = true;
    
    public Action? OnGamePlayed { get; set; }

    public MinesweeperGameView()
    {
        InitializeComponent();
        CreateGrid();
        ResetGame();
    }

    private void CreateGrid()
    {
        for (int i = 0; i < Size; i++)
        {
            GameGrid.ColumnDefinitions.Add(new ColumnDefinition(45));
            GameGrid.RowDefinitions.Add(new RowDefinition(45));
        }

        for (int r = 0; r < Size; r++)
        {
            for (int c = 0; c < Size; c++)
            {
                var row = r;
                var col = c;
                _cells[r, c] = new Button
                {
                    BackgroundColor = Color.FromArgb("#434C5E"),
                    TextColor = Colors.White,
                    FontSize = 16,
                    CornerRadius = 6,
                    Padding = 0
                };
                _cells[r, c].Clicked += (s, e) => OnCellClicked(row, col);
                GameGrid.Add(_cells[r, c], c, r);
            }
        }
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
                _cells[r, c].BackgroundColor = Color.FromArgb("#434C5E");
                _cells[r, c].IsEnabled = true;
            }
        }

        StatusLabel.Text = AppStrings.GetString("FindSafeCells", Mines);
    }

    private void OnResetClicked(object? sender, EventArgs e) => ResetGame();

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
            _cells[row, col].Text = "*";
            _cells[row, col].BackgroundColor = Color.FromArgb("#BF616A");
            GameOver(false);
            return;
        }

        int count = CountAdjacentMines(row, col);
        _cells[row, col].BackgroundColor = Color.FromArgb("#3B4252");

        if (count > 0)
        {
            _cells[row, col].Text = count.ToString();
            _cells[row, col].TextColor = count switch
            {
                1 => Color.FromArgb("#81A1C1"),
                2 => Color.FromArgb("#A3BE8C"),
                3 => Color.FromArgb("#BF616A"),
                _ => Color.FromArgb("#D08770")
            };
        }
        else
        {
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
        StatusLabel.Text = won ? AppStrings.GetString("YouWin") : AppStrings.GetString("BoomGameOver");

        for (int r = 0; r < Size; r++)
            for (int c = 0; c < Size; c++)
                if (_isMine[r, c] && !_revealed[r, c])
                {
                    _cells[r, c].Text = "X";
                    _cells[r, c].BackgroundColor = Color.FromArgb("#BF616A");
                }
    }

    public void Stop() => _gameOver = true;
}
