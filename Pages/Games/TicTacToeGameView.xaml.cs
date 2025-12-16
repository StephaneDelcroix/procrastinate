using procrastinate.Resources.Strings;

namespace procrastinate.Pages.Games;

public partial class TicTacToeGameView : ContentView
{
    private readonly Button[] _cells = new Button[9];
    private readonly string[] _board = new string[9];
    private bool _playerTurn = true;
    private bool _gameOver;
    
    public Action? OnGamePlayed { get; set; }

    public TicTacToeGameView()
    {
        InitializeComponent();
        CreateCells();
        ResetGame();
    }

    private void CreateCells()
    {
        for (int i = 0; i < 9; i++)
        {
            var idx = i;
            _cells[i] = new Button
            {
                BackgroundColor = Color.FromArgb("#334155"),
                TextColor = Colors.White,
                FontSize = 28,
                FontAttributes = FontAttributes.Bold,
                CornerRadius = 8
            };
            _cells[i].Clicked += (s, e) => OnCellClicked(idx);
            GameGrid.Add(_cells[i], i % 3, i / 3);
        }
    }

    private void ResetGame()
    {
        OnGamePlayed?.Invoke();
        for (int i = 0; i < 9; i++)
        {
            _board[i] = "";
            _cells[i].Text = "";
            _cells[i].IsEnabled = true;
        }
        _playerTurn = true;
        _gameOver = false;
        StatusLabel.Text = AppStrings.GetString("YourTurn");
    }

    private void OnResetClicked(object? sender, EventArgs e) => ResetGame();

    private async void OnCellClicked(int idx)
    {
        if (_gameOver || !string.IsNullOrEmpty(_board[idx])) return;

        _board[idx] = "X";
        _cells[idx].Text = "X";
        _cells[idx].TextColor = Color.FromArgb("#14B8A6");

        if (CheckWin("X"))
        {
            StatusLabel.Text = AppStrings.GetString("YouWin");
            _gameOver = true;
            return;
        }

        if (IsBoardFull())
        {
            StatusLabel.Text = AppStrings.GetString("Draw");
            _gameOver = true;
            return;
        }

        _playerTurn = false;
        StatusLabel.Text = AppStrings.GetString("AIThinking");
        await Task.Delay(500);

        var empty = Enumerable.Range(0, 9).Where(i => string.IsNullOrEmpty(_board[i])).ToList();
        if (empty.Count > 0)
        {
            var aiMove = empty[Random.Shared.Next(empty.Count)];
            _board[aiMove] = "O";
            _cells[aiMove].Text = "O";
            _cells[aiMove].TextColor = Color.FromArgb("#EF4444");

            if (CheckWin("O"))
            {
                StatusLabel.Text = AppStrings.GetString("AIWins");
                _gameOver = true;
                return;
            }

            if (IsBoardFull())
            {
                StatusLabel.Text = AppStrings.GetString("Draw");
                _gameOver = true;
                return;
            }
        }

        _playerTurn = true;
        StatusLabel.Text = AppStrings.GetString("YourTurn");
    }

    private bool CheckWin(string player)
    {
        int[,] wins = { {0,1,2}, {3,4,5}, {6,7,8}, {0,3,6}, {1,4,7}, {2,5,8}, {0,4,8}, {2,4,6} };
        for (int i = 0; i < 8; i++)
            if (_board[wins[i,0]] == player && _board[wins[i,1]] == player && _board[wins[i,2]] == player)
                return true;
        return false;
    }

    private bool IsBoardFull() => _board.All(c => !string.IsNullOrEmpty(c));

    public void Stop() => _gameOver = true;
}
