using procrastinate.Services;

namespace procrastinate.Pages;

public partial class ExcusePage : ContentPage
{
    private readonly StatsService _statsService;
    private string _currentExcuse = "";

    private readonly string[] _starters = [
        "I can't do that because",
        "Sorry, but",
        "I would, but",
        "Unfortunately,",
        "I tried, but",
        "You won't believe this, but"
    ];

    private readonly string[] _middles = [
        "my pet goldfish",
        "Mercury is in retrograde and",
        "my horoscope said",
        "a mysterious stranger",
        "my WiFi",
        "my plants",
        "the government",
        "my neighbor's cat",
        "my future self",
        "a ghost"
    ];

    private readonly string[] _endings = [
        "needs me for emotional support.",
        "specifically warned against it.",
        "is judging me too hard right now.",
        "deleted all my motivation.",
        "told me to take a nap instead.",
        "has been acting suspicious lately.",
        "is having an existential crisis.",
        "demanded I watch TV immediately.",
        "ate my to-do list.",
        "said tomorrow would be better."
    ];

    public ExcusePage(StatsService statsService)
    {
        InitializeComponent();
        _statsService = statsService;
    }

    private void OnGenerateClicked(object? sender, EventArgs e)
    {
        var starter = _starters[Random.Shared.Next(_starters.Length)];
        var middle = _middles[Random.Shared.Next(_middles.Length)];
        var ending = _endings[Random.Shared.Next(_endings.Length)];

        _currentExcuse = $"{starter} {middle} {ending}";
        ExcuseLabel.Text = _currentExcuse;
        
        _statsService.IncrementExcusesGenerated();
        CounterLabel.Text = $"Excuses generated today: {_statsService.ExcusesGenerated}";
    }

    private async void OnCopyClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_currentExcuse)) return;
        
        await Clipboard.SetTextAsync(_currentExcuse);
        CopyBtn.Text = "✓ Copied!";
        await Task.Delay(1500);
        CopyBtn.Text = "📋 Copy to Clipboard";
    }
}
