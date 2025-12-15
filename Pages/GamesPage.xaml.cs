using procrastinate.Services;

namespace procrastinate.Pages;

public partial class GamesPage : ContentPage
{
    private readonly StatsService _statsService;
    
    // Simon Says
    private readonly List<int> _simonSequence = [];
    private int _simonIndex;
    private bool _simonPlayerTurn;
    private int _simonScore;
    
    // Click Speed
    private int _clickCount;
    private bool _clickGameActive;
    
    // Reaction Time
    private DateTime _reactionStartTime;
    private bool _reactionWaiting;
    private bool _reactionReady;
    private CancellationTokenSource? _reactionCts;

    public GamesPage(StatsService statsService)
    {
        InitializeComponent();
        _statsService = statsService;
        ClickGameBtn.IsEnabled = false;
        ReactionBtn.IsEnabled = false;
    }

    #region Simon Says
    private async void OnSimonStartClicked(object? sender, EventArgs e)
    {
        _statsService.IncrementGamesPlayed();
        _simonSequence.Clear();
        _simonScore = 0;
        SimonScoreLabel.Text = "Score: 0";
        SimonStartBtn.IsEnabled = false;
        await AddToSimonSequence();
    }

    private async Task AddToSimonSequence()
    {
        _simonSequence.Add(Random.Shared.Next(4));
        await PlaySimonSequence();
    }

    private async Task PlaySimonSequence()
    {
        _simonPlayerTurn = false;
        SetSimonButtonsEnabled(false);
        
        await Task.Delay(500);
        
        foreach (var color in _simonSequence)
        {
            var btn = GetSimonButton(color);
            var originalColor = btn.BackgroundColor;
            btn.BackgroundColor = GetBrightColor(color);
            await Task.Delay(400);
            btn.BackgroundColor = originalColor;
            await Task.Delay(200);
        }
        
        _simonPlayerTurn = true;
        _simonIndex = 0;
        SetSimonButtonsEnabled(true);
    }

    private void OnSimonButtonClicked(object? sender, EventArgs e)
    {
        if (!_simonPlayerTurn || sender is not Button btn) return;
        
        var pressed = GetButtonIndex(btn);
        if (pressed == _simonSequence[_simonIndex])
        {
            _simonIndex++;
            if (_simonIndex >= _simonSequence.Count)
            {
                _simonScore++;
                SimonScoreLabel.Text = $"Score: {_simonScore}";
                _ = AddToSimonSequence();
            }
        }
        else
        {
            SimonScoreLabel.Text = $"Game Over! Final Score: {_simonScore}";
            SimonStartBtn.IsEnabled = true;
            SetSimonButtonsEnabled(false);
        }
    }

    private Button GetSimonButton(int index) => index switch
    {
        0 => RedBtn, 1 => GreenBtn, 2 => BlueBtn, _ => YellowBtn
    };

    private int GetButtonIndex(Button btn)
    {
        if (btn == RedBtn) return 0;
        if (btn == GreenBtn) return 1;
        if (btn == BlueBtn) return 2;
        return 3;
    }

    private static Color GetBrightColor(int index) => index switch
    {
        0 => Colors.Red, 1 => Colors.Lime, 2 => Colors.Blue, _ => Colors.Yellow
    };

    private void SetSimonButtonsEnabled(bool enabled)
    {
        RedBtn.IsEnabled = enabled;
        GreenBtn.IsEnabled = enabled;
        BlueBtn.IsEnabled = enabled;
        YellowBtn.IsEnabled = enabled;
    }
    #endregion

    #region Click Speed
    private async void OnClickStartClicked(object? sender, EventArgs e)
    {
        _statsService.IncrementGamesPlayed();
        _clickCount = 0;
        ClickScoreLabel.Text = "Clicks: 0";
        ClickStartBtn.IsEnabled = false;
        ClickGameBtn.IsEnabled = true;
        _clickGameActive = true;
        
        await Task.Delay(5000);
        
        _clickGameActive = false;
        ClickGameBtn.IsEnabled = false;
        ClickStartBtn.IsEnabled = true;
        ClickScoreLabel.Text = $"Final: {_clickCount} clicks! ({_clickCount / 5.0:F1}/sec)";
    }

    private void OnClickGameClicked(object? sender, EventArgs e)
    {
        if (!_clickGameActive) return;
        _clickCount++;
        ClickScoreLabel.Text = $"Clicks: {_clickCount}";
    }
    #endregion

    #region Reaction Time
    private async void OnReactionStartClicked(object? sender, EventArgs e)
    {
        _statsService.IncrementGamesPlayed();
        _reactionCts?.Cancel();
        _reactionCts = new CancellationTokenSource();
        
        ReactionStartBtn.IsEnabled = false;
        ReactionBtn.IsEnabled = true;
        ReactionBtn.BackgroundColor = Colors.Red;
        ReactionBtn.Text = "Wait...";
        ReactionLabel.Text = "Wait for green!";
        _reactionWaiting = true;
        _reactionReady = false;
        
        var delay = Random.Shared.Next(2000, 5000);
        try
        {
            await Task.Delay(delay, _reactionCts.Token);
            _reactionWaiting = false;
            _reactionReady = true;
            _reactionStartTime = DateTime.Now;
            ReactionBtn.BackgroundColor = Colors.Green;
            ReactionBtn.Text = "TAP NOW!";
        }
        catch (TaskCanceledException) { }
    }

    private void OnReactionClicked(object? sender, EventArgs e)
    {
        if (_reactionWaiting)
        {
            _reactionCts?.Cancel();
            ReactionLabel.Text = "Too early! Try again.";
            ReactionBtn.BackgroundColor = Colors.Orange;
            ReactionBtn.Text = "Too soon!";
            ReactionStartBtn.IsEnabled = true;
            ReactionBtn.IsEnabled = false;
        }
        else if (_reactionReady)
        {
            var reactionTime = (DateTime.Now - _reactionStartTime).TotalMilliseconds;
            ReactionLabel.Text = $"Reaction time: {reactionTime:F0}ms";
            ReactionBtn.BackgroundColor = Colors.Gray;
            ReactionBtn.Text = "Done!";
            ReactionStartBtn.IsEnabled = true;
            ReactionBtn.IsEnabled = false;
            _reactionReady = false;
        }
    }
    #endregion
}
