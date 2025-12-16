using procrastinate.Resources.Strings;

namespace procrastinate.Pages.Games;

public class EggsCatchGame : MiniGame
{
    public override string Name => AppStrings.GetString("EggsCatch");
    public override string Icon => "\uf7fb"; // egg icon
    public override string IconColor => "#FBBF24";
    public override string Description => AppStrings.GetString("EggsCatchDesc");

    private EggsCatchGameView? _gameView;

    public override View CreateGameView()
    {
        _gameView = new EggsCatchGameView
        {
            OnGamePlayed = () => OnGamePlayed?.Invoke()
        };
        return _gameView;
    }

    public override void StartGame() { }
    public override void StopGame() => _gameView?.Stop();
}
