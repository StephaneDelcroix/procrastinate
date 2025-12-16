using Microsoft.Maui.Controls.Shapes;
using procrastinate.Pages.Games;
using procrastinate.Resources.Strings;
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
            new WhackAMoleGame(),
            new EggsCatchGame(),
            new MinifigGeneratorGame()
        ];

        foreach (var game in _allGames)
            game.OnGamePlayed = () => _statsService.IncrementGamesPlayed();

        UpdateShuffleButton();
        ShuffleGames();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateShuffleButton();
    }

    private void UpdateShuffleButton()
    {
        ShuffleBtn.Text = $"\uf074  {AppStrings.GetString("ShuffleGames")}";
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }

    private void OnShuffleClicked(object? sender, EventArgs e)
    {
        foreach (var game in _currentGames)
            game.StopGame();

        ShuffleGames();
    }

    private void ShuffleGames()
    {
        _currentGames.Clear();
        GamesContainer.Children.Clear();

        var weightedGames = new List<MiniGame>();
        foreach (var game in _allGames)
        {
            weightedGames.Add(game);
            if (game.IsFavorite)
            {
                weightedGames.Add(game);
                weightedGames.Add(game);
            }
        }

        var selected = new List<MiniGame>();
        var shuffled = weightedGames.OrderBy(_ => Random.Shared.Next()).ToList();
        foreach (var game in shuffled)
        {
            if (!selected.Contains(game))
            {
                selected.Add(game);
                if (selected.Count >= 3) break;
            }
        }

        _currentGames.AddRange(selected);

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
            TextColor = Color.FromArgb(game.IconColor),
            VerticalOptions = LayoutOptions.Center
        };

        var titleLabel = new Label
        {
            Text = game.Name,
            FontSize = 20,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#F1F5F9"),
            VerticalOptions = LayoutOptions.Center
        };

        var favBtn = new Button
        {
            Text = game.IsFavorite ? "\uf004" : "\uf08a",
            FontFamily = "FontAwesomeSolid",
            FontSize = 18,
            BackgroundColor = Colors.Transparent,
            TextColor = game.IsFavorite ? Color.FromArgb("#EF4444") : Color.FromArgb("#64748B"),
            WidthRequest = 40,
            HeightRequest = 40,
            Padding = 0
        };
        favBtn.Clicked += (s, e) =>
        {
            game.IsFavorite = !game.IsFavorite;
            favBtn.Text = game.IsFavorite ? "\uf004" : "\uf08a";
            favBtn.TextColor = game.IsFavorite ? Color.FromArgb("#EF4444") : Color.FromArgb("#64748B");
        };

        var header = new HorizontalStackLayout
        {
            Spacing = 10,
            HorizontalOptions = LayoutOptions.Center,
            Children = { iconLabel, titleLabel, favBtn }
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
