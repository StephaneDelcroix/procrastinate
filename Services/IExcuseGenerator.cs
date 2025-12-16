namespace procrastinate.Services;

public interface IExcuseGenerator
{
    string Name { get; }
    Task<string> GenerateExcuseAsync(string language);
    bool IsAvailable { get; }
}
