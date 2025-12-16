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
            { "cloud", new CloudExcuseGenerator() },
            { "ondevice", new OnDeviceAIExcuseGenerator() }
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
        { "cloud", "Cloud AI (Groq)" },
        { "ondevice", "On-Device AI (Apple)" }
    };

    public IExcuseGenerator GetCurrentGenerator()
    {
        var mode = CurrentMode;
        return _generators.TryGetValue(mode, out var generator) ? generator : _generators["random"];
    }

    public async Task<string> GenerateExcuseAsync(string language)
    {
        var generator = GetCurrentGenerator();
        var usingAI = (CurrentMode == "cloud" || CurrentMode == "ondevice") && generator.IsAvailable;
        
        // For cloud AI, fallback to random if not available (no API key)
        // For on-device AI, let it generate an error message explaining why
        if (!generator.IsAvailable && CurrentMode == "cloud")
        {
            generator = _generators["random"];
            usingAI = false;
        }
        
        var result = await generator.GenerateExcuseAsync(language);
        
        if (usingAI)
        {
            _stats.IncrementAIExcuseCalls();
        }
        
        return result;
    }
}
