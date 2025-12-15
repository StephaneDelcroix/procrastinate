using Microsoft.Maui.Controls.Shapes;
using procrastinate.Pages.Games;
using procrastinate.Services;

namespace procrastinate.Pages;

public partial class GamesPage : ContentPage
{
    private readonly StatsService _statsService;
    private readonly List<MiniGame> _allGames;
    private readonly List<MiniGame> _currentGames = [];

    public GamesPage(StatsService statsService)
    {
        InitializeComponent();
        _statsService = statsService;

        _allGames =
        [
            new SimonSaysGame(),
            new ClickSpeedGame(),
            new ReactionTimeGame(),
            new TicTacToeGame(),
            new MinesweeperGame(),
            new SnakeGame(),
            new MemoryMatchGame(),
            new NumberGuessingGame(),
            new WhackAMoleGame()
        ];

        foreach (var game in _allGames)
            game.OnGamePlayed = () => _statsService.IncrementGamesPlayed();

        ShuffleGames();
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }

    private void OnShuffleClicked(object? sender, EventArgs e)
    {
        // Stop current games
        foreach (var game in _currentGames)
            game.StopGame();

        ShuffleGames();
    }

    private void ShuffleGames()
    {
        _currentGames.Clear();
        GamesContainer.Children.Clear();

        // Pick 3 random games
        var shuffled = _allGames.OrderBy(_ => Random.Shared.Next()).Take(3).ToList();
        _currentGames.AddRange(shuffled);

        foreach (var game in _currentGames)
        {
            var card = CreateGameCard(game);
            GamesContainer.Children.Add(card);
        }
    }

    private Border CreateGameCard(MiniGame game)
    {
        var iconLabel = new Label
        {
            Text = game.Icon,
            FontFamily = "FontAwesomeSolid",
            FontSize = 20,
            TextColor = Color.FromArgb(game.IconColor)
        };

        var titleLabel = new Label
        {
            Text = game.Name,
            FontSize = 20,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#F1F5F9")
        };

        var header = new HorizontalStackLayout
        {
            Spacing = 10,
            HorizontalOptions = LayoutOptions.Center,
            Children = { iconLabel, titleLabel }
        };

        var descLabel = new Label
        {
            Text = game.Description,
            FontSize = 14,
            TextColor = Color.FromArgb("#94A3B8"),
            HorizontalOptions = LayoutOptions.Center
        };

        var gameView = game.CreateGameView();

        var content = new VerticalStackLayout
        {
            Spacing = 16,
            Children = { header, descLabel, gameView }
        };

        var border = new Border
        {
            BackgroundColor = Color.FromArgb("#1E293B"),
            StrokeShape = new RoundRectangle { CornerRadius = 16 },
            Stroke = Colors.Transparent,
            Padding = 20,
            Content = content,
            Shadow = new Shadow
            {
                Brush = new SolidColorBrush(Color.FromArgb("#40000000")),
                Offset = new Point(0, 4),
                Radius = 12,
                Opacity = 0.3f
            }
        };

        return border;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        foreach (var game in _currentGames)
            game.StopGame();
    }
}
