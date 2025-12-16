using procrastinate.Resources.Strings;

namespace procrastinate.Pages.Games;

public partial class EggsCatchGameView : ContentView
{
    private readonly Label[,] _eggLabels = new Label[3, 4];
    private readonly Label[] _basketLabels = new Label[4];
    private readonly bool[,] _eggs = new bool[3, 4]; // row, column - egg present
    private int _basketPosition = 1; // 0-3 position
    private int _score;
    private int _misses;
    private bool _running;
    private CancellationTokenSource? _cts;
    
    public Action? OnGamePlayed { get; set; }

    public EggsCatchGameView()
    {
        InitializeComponent();
        InitializeLabels();
        UpdateBasket();
    }

    private void InitializeLabels()
    {
        _eggLabels[0, 0] = Egg00; _eggLabels[0, 1] = Egg01; _eggLabels[0, 2] = Egg02; _eggLabels[0, 3] = Egg03;
        _eggLabels[1, 0] = Egg10; _eggLabels[1, 1] = Egg11; _eggLabels[1, 2] = Egg12; _eggLabels[1, 3] = Egg13;
        _eggLabels[2, 0] = Egg20; _eggLabels[2, 1] = Egg21; _eggLabels[2, 2] = Egg22; _eggLabels[2, 3] = Egg23;
        
        _basketLabels[0] = Basket0; _basketLabels[1] = Basket1; _basketLabels[2] = Basket2; _basketLabels[3] = Basket3;
    }

    private void UpdateBasket()
    {
        for (int i = 0; i < 4; i++)
            _basketLabels[i].Text = i == _basketPosition ? "U" : "";
    }

    private void UpdateEggDisplay()
    {
        for (int row = 0; row < 3; row++)
            for (int col = 0; col < 4; col++)
                _eggLabels[row, col].Text = _eggs[row, col] ? "O" : "";
    }

    private void OnLeftClicked(object? sender, EventArgs e)
    {
        if (_basketPosition > 0)
        {
            _basketPosition--;
            UpdateBasket();
        }
    }

    private void OnRightClicked(object? sender, EventArgs e)
    {
        if (_basketPosition < 3)
        {
            _basketPosition++;
            UpdateBasket();
        }
    }

    private async void OnStartClicked(object? sender, EventArgs e)
    {
        if (_running) return;
        OnGamePlayed?.Invoke();

        _score = 0;
        _misses = 0;
        _basketPosition = 1;
        _running = true;
        _cts = new CancellationTokenSource();
        StartBtn.IsEnabled = false;
        LeftBtn.IsEnabled = true;
        RightBtn.IsEnabled = true;
        
        // Clear all eggs
        for (int row = 0; row < 3; row++)
            for (int col = 0; col < 4; col++)
                _eggs[row, col] = false;
        
        UpdateBasket();
        UpdateEggDisplay();
        UpdateScore();

        await GameLoop();
    }

    private async Task GameLoop()
    {
        int speed = 600; // ms between updates
        int spawnChance = 30; // % chance to spawn egg in each column

        try
        {
            while (_running && _misses < 3 && !_cts!.Token.IsCancellationRequested)
            {
                await Task.Delay(speed, _cts.Token);

                // Move eggs down
                for (int col = 0; col < 4; col++)
                {
                    // Check if bottom egg reaches basket row
                    if (_eggs[2, col])
                    {
                        if (_basketPosition == col)
                        {
                            // Caught!
                            _score++;
                        }
                        else
                        {
                            // Missed!
                            _misses++;
                        }
                        _eggs[2, col] = false;
                    }

                    // Shift eggs down
                    for (int row = 2; row > 0; row--)
                        _eggs[row, col] = _eggs[row - 1, col];
                    
                    _eggs[0, col] = false;
                }

                // Spawn new eggs at top (random columns)
                for (int col = 0; col < 4; col++)
                {
                    if (Random.Shared.Next(100) < spawnChance)
                        _eggs[0, col] = true;
                }

                UpdateEggDisplay();
                UpdateScore();

                // Speed up as score increases
                speed = Math.Max(200, 600 - (_score * 20));
                spawnChance = Math.Min(50, 30 + (_score / 5));
            }
        }
        catch (TaskCanceledException) { }

        _running = false;
        StartBtn.IsEnabled = true;
        LeftBtn.IsEnabled = false;
        RightBtn.IsEnabled = false;
        ScoreLabel.Text = AppStrings.GetString("GameOverScore", _score);
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
