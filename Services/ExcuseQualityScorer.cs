using Microsoft.Extensions.AI;

namespace procrastinate.Services;

/// <summary>
/// Uses NLEmbeddingGenerator (IEmbeddingGenerator) to score excuse quality
/// by comparing against a set of known-good excuses via cosine similarity.
/// </summary>
public static class ExcuseQualityScorer
{
    private static readonly string[] GoldenExcuses =
    {
        "I can't come in today because my pet goldfish is having an existential crisis and needs emotional support.",
        "Sorry, but Mercury is in retrograde and my horoscope specifically warned against productivity.",
        "I would, but a mysterious stranger told me that if I do any work today the timeline collapses.",
        "I tried, but my WiFi became sentient and is now holding my files hostage.",
        "I'd love to help, but my neighbor's cat has filed a restraining order against my productivity.",
        "Unfortunately, I accidentally invented time travel and I'm stuck in yesterday.",
        "I must decline because a fortune cookie predicted catastrophe if I'm productive today.",
        "Not today — my shadow called in sick and I can't function without it.",
        "I was going to, but my coffee achieved consciousness and we need to have a talk.",
        "Sorry, a philosophical duck won't leave my porch and I need to hear its argument first."
    };

    /// <summary>
    /// Score an excuse against golden examples. Returns 0.0 to 1.0 (higher = more similar to good excuses).
    /// </summary>
    public static async Task<float> ScoreAsync(
        IEmbeddingGenerator<string, Embedding<float>>? embeddingGenerator,
        string excuse)
    {
        if (embeddingGenerator is null || string.IsNullOrWhiteSpace(excuse))
            return 0.5f; // neutral score when embeddings unavailable

        try
        {
            // Get embedding for the generated excuse
            var excuseEmbeddings = await embeddingGenerator.GenerateAsync([excuse]);
            var excuseVector = excuseEmbeddings[0].Vector;

            // Get embeddings for golden excuses
            var goldenEmbeddings = await embeddingGenerator.GenerateAsync(GoldenExcuses.ToList());

            // Compute max cosine similarity against golden set
            float maxSimilarity = 0f;
            foreach (var golden in goldenEmbeddings)
            {
                var similarity = CosineSimilarity(excuseVector, golden.Vector);
                if (similarity > maxSimilarity)
                    maxSimilarity = similarity;
            }

            return maxSimilarity;
        }
        catch
        {
            return 0.5f; // fallback on error
        }
    }

    private static float CosineSimilarity(ReadOnlyMemory<float> a, ReadOnlyMemory<float> b)
    {
        var spanA = a.Span;
        var spanB = b.Span;
        var len = Math.Min(spanA.Length, spanB.Length);

        float dot = 0, magA = 0, magB = 0;
        for (int i = 0; i < len; i++)
        {
            dot += spanA[i] * spanB[i];
            magA += spanA[i] * spanA[i];
            magB += spanB[i] * spanB[i];
        }

        var denom = MathF.Sqrt(magA) * MathF.Sqrt(magB);
        return denom == 0 ? 0 : dot / denom;
    }
}
