using System.Diagnostics;
using Microsoft.Extensions.AI;

namespace procrastinate.Services;

/// <summary>
/// Multi-agent pipeline using Apple Intelligence for research and embedded ONNX models for writing/editing:
/// Apple Intelligence (researcher) → Llama 3.2 3B (writer) → Llama 3.2 1B (editor).
/// </summary>
public class EmbeddedAgentPipelineExcuseGenerator : IExcuseGenerator
{
    private const string WriterModelId = "llama-3.2-3b-int4";
    private const string EditorModelId = "llama-3.2-1b-int4";

    private readonly IChatClient? _onDeviceChatClient;

    public string Name => "Embedded Agent Pipeline";

    public bool IsAvailable =>
        _onDeviceChatClient is not null &&
        OnnxModelManager.IsModelDownloaded(WriterModelId) &&
        OnnxModelManager.IsModelDownloaded(EditorModelId);

    public Action<string>? OnStageChanged { get; set; }
    public Action<string, string>? OnAgentOutput { get; set; }

    public EmbeddedAgentPipelineExcuseGenerator(IChatClient? onDeviceChatClient = null)
    {
        _onDeviceChatClient = onDeviceChatClient;
    }

    public async Task<ExcuseResult> GenerateExcuseAsync(string language)
    {
        var stopwatch = Stopwatch.StartNew();

        if (_onDeviceChatClient is null)
        {
            stopwatch.Stop();
            return new ExcuseResult(
                "Apple Intelligence is required for the researcher agent. Use a device with iOS 18.4+.",
                Name, stopwatch.Elapsed);
        }

        if (!OnnxModelManager.IsModelDownloaded(WriterModelId) || !OnnxModelManager.IsModelDownloaded(EditorModelId))
        {
            var missing = new List<string>();
            if (!OnnxModelManager.IsModelDownloaded(WriterModelId)) missing.Add("Llama 3.2 3B");
            if (!OnnxModelManager.IsModelDownloaded(EditorModelId)) missing.Add("Llama 3.2 1B");
            stopwatch.Stop();
            return new ExcuseResult(
                $"Download models in Settings → Embedded ONNX: {string.Join(", ", missing)}",
                Name, stopwatch.Elapsed);
        }

        try
        {
            var languageName = GetLanguageName(language);

            // Agent 1: Researcher (Apple Intelligence) — picks an absurd scenario
            OnStageChanged?.Invoke("🔍 Apple Intelligence: Researching...");
            var researchMessages = new List<ChatMessage>
            {
                new(ChatRole.User, $"Write a short funny fictional story about {GetRandomElement()}. Make it silly and absurd. Maximum 2 sentences.")
            };
            using var researchCts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var researchResponse = await _onDeviceChatClient.GetResponseAsync(researchMessages, cancellationToken: researchCts.Token);
            var scenario = researchResponse.Text?.Trim() ?? "";
            // Truncate to keep within Llama's context window
            if (scenario.Length > 300)
                scenario = scenario[..300];
            OnAgentOutput?.Invoke("🔍 Apple Intelligence", scenario);

            // Agent 2: Writer (Llama 3.2 3B) — crafts the excuse
            OnStageChanged?.Invoke("✍️ Llama 3B: Writing...");
            var rawExcuse = await RunAgentAsync(WriterModelId,
                "You turn scenarios into short funny first-person excuses. 1-2 sentences max.",
                $"Scenario: {scenario}\n\nWrite a funny excuse in {languageName} based on that. Start with 'Sorry'. 1-2 sentences only.");
            OnAgentOutput?.Invoke("✍️ Llama 3B Writer", rawExcuse);

            // Agent 3: Editor (Llama 3.2 1B) — polishes
            OnStageChanged?.Invoke("✨ Llama 1B: Polishing...");
            var finalExcuse = await RunAgentAsync(EditorModelId,
                "You polish excuses to be funnier. Keep it short. Same language.",
                $"Polish this excuse in {languageName}, keep 1-2 sentences:\n\n{rawExcuse}");
            OnAgentOutput?.Invoke("✨ Llama 1B Editor", finalExcuse.Trim());

            OnStageChanged?.Invoke("✅ Done!");
            stopwatch.Stop();

            return new ExcuseResult(
                finalExcuse.Trim(),
                Name,
                stopwatch.Elapsed,
                Model: "Apple Intelligence → Llama 3B → Llama 1B");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Debug.WriteLine($"Embedded pipeline error: {ex}");
            return new ExcuseResult($"Embedded pipeline error: {ex.Message}", Name, stopwatch.Elapsed);
        }
    }

    private static async Task<string> RunAgentAsync(string modelId, string systemPrompt, string userPrompt)
    {
        var modelPath = OnnxModelManager.GetModelDirectory(modelId);
        var modelInfo = OnnxModelManager.AvailableModels.First(m => m.Id == modelId);
        var client = OnnxGenAIChatClient.GetOrCreate(modelPath, modelInfo.Name, modelInfo.Template);

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, systemPrompt),
            new(ChatRole.User, userPrompt)
        };

        var response = await client.GetResponseAsync(messages, new ChatOptions
        {
            MaxOutputTokens = 256,
            Temperature = 0.9f
        });

        return response.Text?.Trim() ?? "";
    }

    private static string GetRandomElement()
    {
        var elements = new[]
        {
            "a time-traveling pigeon", "sentient office furniture", "a secret society of squirrels",
            "a parallel universe where gravity is optional", "a conspiracy involving socks",
            "an AI that became a life coach", "a haunted coffee machine", "quantum entangled twins",
            "a diplomatic incident with a penguin", "a rogue weather satellite",
            "an enchanted parking meter", "a philosophical debate with a cat",
            "a mysterious portal in the closet", "an accidental invention", "a cursed alarm clock",
            "a runaway sourdough starter", "an overly helpful robot vacuum",
            "a neighborhood raccoon uprising", "a telepathic houseplant", "a glitch in spacetime"
        };
        return elements[Random.Shared.Next(elements.Length)];
    }

    private static string GetLanguageName(string language) => language switch
    {
        "fr" => "French",
        "es" => "Spanish",
        "pt" => "Portuguese",
        "nl" => "Dutch",
        "cs" => "Czech",
        "uk" => "Ukrainian",
        _ => "English"
    };
}
