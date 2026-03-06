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
                new(ChatRole.System, "You are a creative comedy researcher. Your job is to come up with an absurd, unexpected scenario that could be used as an excuse. Output ONLY the scenario in 1-2 sentences. Be wildly creative."),
                new(ChatRole.User, $"Come up with a bizarre, funny scenario involving {GetRandomElement()}. Make it unexpected and absurd. Just the scenario, nothing else.")
            };
            var researchResponse = await _onDeviceChatClient.GetResponseAsync(researchMessages);
            var scenario = researchResponse.Text?.Trim() ?? "";
            OnAgentOutput?.Invoke("🔍 Apple Intelligence", scenario);

            // Agent 2: Writer (Llama 3.2 3B) — crafts the excuse
            OnStageChanged?.Invoke("✍️ Llama 3B: Writing...");
            var rawExcuse = await RunAgentAsync(WriterModelId,
                "You are a comedy writer. You turn scenarios into first-person excuses that sound like something a real person would say. Keep it to 1-2 sentences. Start with 'I' or 'Sorry'.",
                $"Turn this scenario into a funny first-person excuse in {languageName}:\n\n{scenario}\n\nJust the excuse, nothing else.");
            OnAgentOutput?.Invoke("✍️ Llama 3B Writer", rawExcuse);

            // Agent 3: Editor (Llama 3.2 1B) — polishes
            OnStageChanged?.Invoke("✨ Llama 1B: Polishing...");
            var finalExcuse = await RunAgentAsync(EditorModelId,
                $"You are a comedy editor. You polish excuses to be funnier and more natural-sounding. Keep the same language ({languageName}). Output ONLY the polished excuse.",
                $"Polish this excuse to be funnier and more natural. Keep it in {languageName}. Keep it to 1-2 sentences:\n\n{rawExcuse}\n\nJust the polished excuse, nothing else.");
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
        var client = OnnxGenAIChatClient.GetOrCreate(modelPath, modelInfo.Name);

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, systemPrompt),
            new(ChatRole.User, userPrompt)
        };

        var response = await client.GetResponseAsync(messages, new ChatOptions
        {
            MaxOutputTokens = 150,
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
