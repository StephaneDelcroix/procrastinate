using procrastinate.Resources.Strings;

namespace procrastinate.Pages.Games;

public class MinifigGeneratorGame : MiniGame
{
    public override string Name => AppStrings.GetString("MinifigGenerator");
    public override string Icon => "\uf1ae"; // child/person icon
    public override string IconColor => "#F59E0B";
    public override string Description => AppStrings.GetString("MinifigGeneratorDesc");

    private MinifigGeneratorGameView? _gameView;

    public override View CreateGameView()
    {
        _gameView = new MinifigGeneratorGameView
        {
            OnGamePlayed = () => OnGamePlayed?.Invoke()
        };
        return _gameView;
    }

    public override void StartGame() { }
    public override void StopGame() => _gameView?.Stop();
}
