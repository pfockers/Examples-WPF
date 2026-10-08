namespace DependencyInjectionWpf.Services;

public sealed class GreetingService : IGreetingService
{
    public string CreateGreeting(string name)
    {
        var displayName = string.IsNullOrWhiteSpace(name) ? "guest" : name.Trim();
        return $"Hello, {displayName}! This greeting came from an injected service.";
    }
}
