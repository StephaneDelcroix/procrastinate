namespace procrastinate.Services;

public class ExcuseService
{
    private readonly Dictionary<string, IExcuseGenerator> _generators;
    private readonly StatsService _stats;
    
    public ExcuseService(StatsService stats)
    {
        _stats = stats;
        _generators = new Dictionary<string, IExcuseGenerator>
        {
            { "random", new RandomExcuseGenerator() },
            { "cloud", new CloudExcuseGenerator() }
        };
    }

    public static string CurrentMode
    {
        get => Preferences.Get("ExcuseMode", "random");
        set => Preferences.Set("ExcuseMode", value);
    }

    public static readonly Dictionary<string, string> AvailableModes = new()
    {
        { "random", "Random Generator" },
        { "cloud", "Cloud AI (Groq)" }
    };

    public IExcuseGenerator GetCurrentGenerator()
    {
        var mode = CurrentMode;
        return _generators.TryGetValue(mode, out var generator) ? generator : _generators["random"];
    }

    public async Task<string> GenerateExcuseAsync(string language)
    {
        var generator = GetCurrentGenerator();
        var usingCloud = CurrentMode == "cloud" && generator.IsAvailable;
        
        // Fallback to random if cloud is not available
        if (!generator.IsAvailable && CurrentMode == "cloud")
        {
            generator = _generators["random"];
            usingCloud = false;
        }
        
        var result = await generator.GenerateExcuseAsync(language);
        
        if (usingCloud)
        {
            _stats.IncrementAIExcuseCalls();
        }
        
        return result;
    }
}
