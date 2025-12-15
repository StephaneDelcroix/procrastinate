namespace procrastinate.Services;

public class StatsService
{
    private int _tasksAvoided;
    private int _excusesGenerated;
    private int _gamesPlayed;
    private int _breaksTaken;

    public int TasksAvoided => _tasksAvoided;
    public int ExcusesGenerated => _excusesGenerated;
    public int GamesPlayed => _gamesPlayed;
    public int BreaksTaken => _breaksTaken;

    public void IncrementTasksAvoided() => _tasksAvoided++;
    public void IncrementExcusesGenerated() => _excusesGenerated++;
    public void IncrementGamesPlayed() => _gamesPlayed++;
    public void IncrementBreaksTaken() => _breaksTaken++;
}
