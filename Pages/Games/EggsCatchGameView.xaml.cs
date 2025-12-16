using procrastinate.Resources.Strings;

namespace procrastinate.Pages.Games;

public partial class EggsCatchGameView : ContentView
{
    private readonly Label[,] _eggLabels = new Label[3, 4];
    private readonly Label[] _basketLabels = new Label[4];
    private readonly int[] _eggRows = new int[4]; // -1 = no egg, 0-2 = row position
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
        ClearEggs();
    }

    private void InitializeLabels()
    {
        _eggLabels[0, 0] = Egg00; _eggLabels[0, 1] = Egg01; _eggLabels[0, 2] = Egg02; _eggLabels[0, 3] = Egg03;
        _eggLabels[1, 0] = Egg10; _eggLabels[1, 1] = Egg11; _eggLabels[1, 2] = Egg12; _eggLabels[1, 3] = Egg13;
        _eggLabels[2, 0] = Egg20; _eggLabels[2, 1] = Egg21; _eggLabels[2, 2] = Egg22; _eggLabels[2, 3] = Egg23;
        
        _basketLabels[0] = Basket0; _basketLabels[1] = Basket1; _basketLabels[2] = Basket2; _basketLabels[3] = Basket3;
    }

    private void ClearEggs()
    {
        for (int col = 0; col < 4; col++)
            _eggRows[col] = -1;
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
                _eggLabels[row, col].Text = _eggRows[col] == row ? "O" : "";
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
        
        ClearEggs();
        UpdateBasket();
        UpdateEggDisplay();
        UpdateScore();

        await GameLoop();
    }

    private async Task GameLoop()
    {
        int speed = 500;

        try
        {
            while (_running && _misses < 3 && !_cts!.Token.IsCancellationRequested)
            {
                await Task.Delay(speed, _cts.Token);

                // Count eggs about to land (row 2)
                int eggsLanding = 0;
                int landingCol = -1;
                for (int col = 0; col < 4; col++)
                {
                    if (_eggRows[col] == 2)
                    {
                        eggsLanding++;
                        landingCol = col;
                    }
                }

                // Move eggs down, but stagger landings
                for (int col = 0; col < 4; col++)
                {
                    if (_eggRows[col] == 2)
                    {
                        // Only let ONE egg land per tick
                        if (eggsLanding > 1 && col != landingCol)
                        {
                            // Hold this egg, it lands next tick
                            continue;
                        }
                        
                        // Egg reaches bottom
                        if (_basketPosition == col)
                        {
                            _score++;
                        }
                        else
                        {
                            _misses++;
                        }
                        _eggRows[col] = -1;
                    }
                    else if (_eggRows[col] >= 0)
                    {
                        _eggRows[col]++;
                    }
                }

                // Spawn new egg in a random empty column (only spawn one at a time)
                var emptyCols = new List<int>();
                for (int col = 0; col < 4; col++)
                    if (_eggRows[col] == -1)
                        emptyCols.Add(col);
                
                if (emptyCols.Count > 0 && Random.Shared.Next(100) < 40)
                {
                    int spawnCol = emptyCols[Random.Shared.Next(emptyCols.Count)];
                    _eggRows[spawnCol] = 0;
                }

                UpdateEggDisplay();
                UpdateScore();

                // Speed up as score increases
                speed = Math.Max(250, 500 - (_score * 15));
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
