namespace procrastinate.Services;

public class StatsService
{
    private int _tasksAvoided;
    private int _excusesGenerated;
    private int _gamesPlayed;
    private int _breaksTaken;
    private int _aiExcuseCalls;
    private readonly Dictionary<string, int> _gameHighScores = [];

    public int TasksAvoided => _tasksAvoided;
    public int ExcusesGenerated => _excusesGenerated;
    public int GamesPlayed => _gamesPlayed;
    public int BreaksTaken => _breaksTaken;
    public int AIExcuseCalls => _aiExcuseCalls;
    public IReadOnlyDictionary<string, int> GameHighScores => _gameHighScores;

    public void IncrementTasksAvoided() => _tasksAvoided++;
    public void IncrementExcusesGenerated() => _excusesGenerated++;
    public void IncrementGamesPlayed() => _gamesPlayed++;
    public void IncrementBreaksTaken() => _breaksTaken++;
    public void IncrementAIExcuseCalls() => _aiExcuseCalls++;

    public void UpdateHighScore(string gameName, int score)
    {
        if (!_gameHighScores.TryGetValue(gameName, out var current) || score > current)
        {
            _gameHighScores[gameName] = score;
        }
    }

    public int GetHighScore(string gameName) => _gameHighScores.TryGetValue(gameName, out var score) ? score : 0;
}
