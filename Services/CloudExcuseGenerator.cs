using System.Text.Json;
using System.Text.Json.Serialization;

namespace procrastinate.Services;

public class CloudExcuseGenerator : IExcuseGenerator
{
    public string Name => "Cloud AI (Groq)";
    public bool IsAvailable => !string.IsNullOrEmpty(ApiKey);
    
    private static string ApiKey => Preferences.Get("GroqApiKey", "");
    private static string ApiEndpoint => Preferences.Get("GroqApiEndpoint", "https://api.groq.com/openai/v1/chat/completions");
    private static string Model => Preferences.Get("GroqModel", "llama-3.3-70b-versatile");
    private const int MaxRetries = 3;
    
    private static readonly Lazy<HttpClient> _httpClient = new(() => 
        new HttpClient { Timeout = TimeSpan.FromSeconds(30) });

    public async Task<string> GenerateExcuseAsync(string language)
    {
        if (!IsAvailable)
            return "Cloud AI requires an API key. Configure it in Settings.";

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

            var request = new GroqRequest
            {
                Model = Model,
                Messages = new[]
                {
                    new GroqMessage { Role = "user", Content = prompt }
                }
            };

            var json = JsonSerializer.Serialize(request);
            
            Exception? lastException = null;
            for (int retry = 0; retry < MaxRetries; retry++)
            {
                try
                {
                    var httpRequest = new HttpRequestMessage(HttpMethod.Post, ApiEndpoint)
                    {
                        Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
                    };
                    httpRequest.Headers.Add("Authorization", $"Bearer {ApiKey}");
                    
                    var response = await _httpClient.Value.SendAsync(httpRequest);
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorBody = await response.Content.ReadAsStringAsync();
                        return $"API error ({response.StatusCode}): {errorBody}";
                    }

                    var responseBody = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<GroqResponse>(responseBody);
                    
                    return result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim() ?? "The AI is also procrastinating...";
                }
                catch (Exception ex) when (retry < MaxRetries - 1)
                {
                    lastException = ex;
                    await Task.Delay(1000 * (retry + 1)); // Exponential backoff
                }
            }
            
            return $"Connection failed: {lastException?.Message ?? "Unknown error"}";
        }
        catch (Exception ex)
        {
            return $"Cloud excuse failed: {ex.Message}";
        }
    }

    private class GroqRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = "";
        [JsonPropertyName("messages")]
        public GroqMessage[] Messages { get; set; } = Array.Empty<GroqMessage>();
    }

    private class GroqMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = "";
        [JsonPropertyName("content")]
        public string Content { get; set; } = "";
    }

    private class GroqResponse
    {
        [JsonPropertyName("choices")]
        public GroqChoice[]? Choices { get; set; }
    }

    private class GroqChoice
    {
        [JsonPropertyName("message")]
        public GroqMessage? Message { get; set; }
    }
}
