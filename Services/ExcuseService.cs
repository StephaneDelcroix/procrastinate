using Microsoft.Extensions.AI;

namespace procrastinate.Services;

public class ExcuseService
{
    private readonly StatsService _stats;
    private readonly IChatClient? _onDeviceChatClient;
    private readonly RandomExcuseGenerator _randomGenerator = new();

    public ExcuseService(StatsService stats, IChatClient? onDeviceChatClient = null)
    {
        _stats = stats;
        _onDeviceChatClient = onDeviceChatClient;
    }

    public static string CurrentMode
    {
        get => Preferences.Get("ExcuseMode", "random");
        set => Preferences.Set("ExcuseMode", value);
    }

    public static Dictionary<string, string> AvailableModes
    {
        get
        {
            var modes = new Dictionary<string, string>
            {
                { "random", "Random Generator" },
                { "cloud", "Cloud AI (MEAI + Groq)" }
            };

#if IOS || MACCATALYST
            modes.Add("ondevice", "On-Device AI (MEAI + Apple Intelligence)");
#endif
            return modes;
        }
    }

    /// <summary>
    /// Returns true if on-device AI (Apple Intelligence via MEAI) is available.
    /// </summary>
    public bool IsOnDeviceAvailable => _onDeviceChatClient is not null;

    /// <summary>
    /// Returns the IChatClient for the currently selected AI mode, or null for random.
    /// </summary>
    public IChatClient? GetCurrentChatClient()
    {
        return CurrentMode switch
        {
            "ondevice" => _onDeviceChatClient,
            "cloud" when CloudExcuseGenerator.IsCloudAvailable => CloudExcuseGenerator.CreateChatClient(),
            _ => null
        };
    }

    public IExcuseGenerator GetCurrentGenerator()
    {
        return CurrentMode switch
        {
            "cloud" => new CloudExcuseGenerator(),
            "ondevice" => new OnDeviceAIExcuseGenerator(_onDeviceChatClient),
            _ => _randomGenerator
        };
    }

    public async Task<ExcuseResult> GenerateExcuseAsync(string language)
    {
        var generator = GetCurrentGenerator();
        var usingAI = (CurrentMode == "cloud" || CurrentMode == "ondevice") && generator.IsAvailable;

        var result = await generator.GenerateExcuseAsync(language);

        if (usingAI)
        {
            _stats.IncrementAIExcuseCalls();
        }

        return result;
    }
}
