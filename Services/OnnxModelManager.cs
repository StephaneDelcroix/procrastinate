using System.Text.Json;

namespace procrastinate.Services;

public record OnnxModelInfo(
    string Id,
    string Name,
    string HuggingFaceRepo,
    string SubFolder,
    long EstimatedSizeBytes);

public class OnnxModelManager
{
    private static readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromMinutes(60) };

    public static readonly OnnxModelInfo[] AvailableModels =
    [
        new("phi3-mini-int4", "Phi-3 Mini INT4 (2.5 GB)",
            "microsoft/Phi-3-mini-4k-instruct-onnx",
            "cpu_and_mobile/cpu-int4-rtn-block-32-acc-level-4",
            2_725_535_000L)
    ];

    public static string GetModelDirectory(string modelId)
        => Path.Combine(FileSystem.AppDataDirectory, "onnx-models", modelId);

    public static bool IsModelDownloaded(string modelId)
    {
        var dir = GetModelDirectory(modelId);
        return Directory.Exists(dir) && File.Exists(Path.Combine(dir, "genai_config.json"));
    }

    public static long GetDownloadedSize(string modelId)
    {
        var dir = GetModelDirectory(modelId);
        if (!Directory.Exists(dir)) return 0;
        return new DirectoryInfo(dir).EnumerateFiles("*", SearchOption.AllDirectories).Sum(f => f.Length);
    }

    public static async Task DownloadModelAsync(
        OnnxModelInfo model,
        IProgress<(long downloaded, long total, string file)>? progress = null,
        CancellationToken ct = default)
    {
        var modelDir = GetModelDirectory(model.Id);
        Directory.CreateDirectory(modelDir);

        // List files from HuggingFace API
        var apiUrl = $"https://huggingface.co/api/models/{model.HuggingFaceRepo}/tree/main/{model.SubFolder}";
        var json = await _httpClient.GetStringAsync(apiUrl, ct);
        var entries = JsonSerializer.Deserialize<JsonElement[]>(json) ?? [];

        var files = new List<(string name, long size)>();
        long totalSize = 0;

        foreach (var entry in entries)
        {
            if (entry.GetProperty("type").GetString() != "file") continue;
            var path = entry.GetProperty("path").GetString()!;
            var name = Path.GetFileName(path);
            if (name.EndsWith(".py")) continue;
            var size = entry.GetProperty("size").GetInt64();
            // For LFS files, use the actual LFS size
            if (entry.TryGetProperty("lfs", out var lfs))
                size = lfs.GetProperty("size").GetInt64();
            files.Add((name, size));
            totalSize += size;
        }

        long downloaded = 0;
        foreach (var (name, size) in files)
        {
            ct.ThrowIfCancellationRequested();
            var filePath = Path.Combine(modelDir, name);

            // Skip already downloaded files with correct size
            if (File.Exists(filePath) && new FileInfo(filePath).Length == size)
            {
                downloaded += size;
                progress?.Report((downloaded, totalSize, $"✓ {name}"));
                continue;
            }

            var url = $"https://huggingface.co/{model.HuggingFaceRepo}/resolve/main/{model.SubFolder}/{name}";
            progress?.Report((downloaded, totalSize, $"⬇ {name}"));

            using var resp = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct);
            resp.EnsureSuccessStatusCode();

            await using var stream = await resp.Content.ReadAsStreamAsync(ct);
            await using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 81920);

            var buffer = new byte[81920];
            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer, ct)) > 0)
            {
                await fs.WriteAsync(buffer.AsMemory(0, bytesRead), ct);
                downloaded += bytesRead;
                progress?.Report((downloaded, totalSize, $"⬇ {name}"));
            }
        }
    }

    public static void DeleteModel(string modelId)
    {
        var dir = GetModelDirectory(modelId);
        if (Directory.Exists(dir))
            Directory.Delete(dir, true);
    }
}
