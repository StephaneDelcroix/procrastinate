using System.Text.Json;

namespace procrastinate.Services;

public class CloudExcuseGenerator : IExcuseGenerator
{
    public string Name => "Cloud AI";
    public bool IsAvailable => !string.IsNullOrEmpty(ApiEndpoint);
    
    private static string ApiEndpoint => Preferences.Get("ExcuseApiEndpoint", "");
    private static readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromSeconds(30) };

    public async Task<string> GenerateExcuseAsync(string language)
    {
        if (!IsAvailable)
            throw new InvalidOperationException("Cloud AI endpoint not configured");

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

            var prompt = $"Generate a single funny, creative excuse for not doing work or being productive. The excuse should be absurd but delivered with a straight face. Write it in {languageName}. Reply with ONLY the excuse text, no quotes or explanation.";

            var request = new
            {
                model = Preferences.Get("ExcuseAiModel", "llama3.2"),
                prompt,
                stream = false
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{ApiEndpoint}/api/generate", content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OllamaResponse>(responseBody);
            
            return result?.response?.Trim() ?? "The AI is also procrastinating...";
        }
        catch (Exception ex)
        {
            return $"Cloud excuse failed: {ex.Message}";
        }
    }

    private class OllamaResponse
    {
        public string? response { get; set; }
    }
}
