using System.Diagnostics;
using Microsoft.Extensions.AI;

namespace procrastinate.Services;

/// <summary>
/// On-device AI excuse generator using Apple Intelligence via MEAI's AppleIntelligenceChatClient (IChatClient).
/// Requires iOS 26+ / macOS 26+ with Apple Intelligence enabled.
/// </summary>
public class OnDeviceAIExcuseGenerator : IExcuseGenerator
{
    private readonly IChatClient? _chatClient;

    public string Name => "On-Device AI (Apple)";

    public OnDeviceAIExcuseGenerator(IChatClient? chatClient = null)
    {
        _chatClient = chatClient;
    }

    public bool IsAvailable => _chatClient is not null;

    public async Task<ExcuseResult> GenerateExcuseAsync(string language)
    {
        var stopwatch = Stopwatch.StartNew();

        if (_chatClient is null)
        {
            stopwatch.Stop();
            return new ExcuseResult("On-device AI is not available on this device.", Name, stopwatch.Elapsed);
        }

        try
        {
            var languageName = language switch
            {
                "fr" => "French",
                "es" => "Spanish",
                "pt" => "Portuguese",
                "nl" => "Dutch",
                "cs" => "Czech",
                _ => "English"
            };

            var prompt = $"Write a single short humorous fictional excuse that starts with 'I' - something silly and absurd a person might say when they're late or can't do something. Make it sound like a real excuse someone would say out loud. Write it in {languageName}. Just the excuse itself, nothing else.";

            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, "You are writing funny excuses in first person. Start naturally like 'I can't because...' or 'I would but...'. Keep it to one or two sentences. Be creative and absurd but make it sound like something a person would actually say."),
                new(ChatRole.User, prompt)
            };

            var response = await _chatClient.GetResponseAsync(messages);
            stopwatch.Stop();

            var excuse = response.Text?.Trim() ?? "The on-device AI is also procrastinating...";

            return new ExcuseResult(excuse, Name, stopwatch.Elapsed, Model: "Apple Intelligence");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new ExcuseResult($"On-device AI error: {ex.Message}", Name, stopwatch.Elapsed);
        }
    }
}
