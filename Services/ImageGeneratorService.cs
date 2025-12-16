using System.Net;

namespace procrastinate.Services;

public class ImageGeneratorService
{
    private static readonly Lazy<HttpClient> _httpClient = new(() => 
        new HttpClient { Timeout = TimeSpan.FromSeconds(60) });

    /// <summary>
    /// Generates a cartoon-style image based on the excuse text using Pollinations.ai (free, no API key)
    /// </summary>
    public async Task<Stream?> GenerateImageAsync(string excuse)
    {
        try
        {
            // Create a prompt for a cartoon-style image
            var imagePrompt = $"Cartoon illustration of: {excuse}. Style: funny, colorful, comic book style, simple, humorous character";
            
            // URL encode the prompt
            var encodedPrompt = WebUtility.UrlEncode(imagePrompt);
            
            // Pollinations.ai free image generation API
            var imageUrl = $"https://image.pollinations.ai/prompt/{encodedPrompt}?width=512&height=512&nologo=true";
            
            var response = await _httpClient.Value.GetAsync(imageUrl);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStreamAsync();
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Gets the URL for image generation (for sharing without downloading)
    /// </summary>
    public string GetImageUrl(string excuse)
    {
        var imagePrompt = $"Cartoon illustration of: {excuse}. Style: funny, colorful, comic book style, simple, humorous character";
        var encodedPrompt = WebUtility.UrlEncode(imagePrompt);
        return $"https://image.pollinations.ai/prompt/{encodedPrompt}?width=512&height=512&nologo=true";
    }
}
