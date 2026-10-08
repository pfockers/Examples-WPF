using Microsoft.Extensions.Logging;

namespace ErrorHandlingLoggingWpf.Services;

public sealed class DemoOperationService(ILogger<DemoOperationService> logger) : IDemoOperationService
{
    public string RunSuccessfully()
    {
        logger.LogInformation("The demo operation completed successfully.");
        return "Operation completed successfully. An information log was written.";
    }

    public void ThrowHandledFailure() =>
        throw new InvalidOperationException("This is a simulated recoverable operation failure.");

    public void ThrowUnhandledFailure() =>
        throw new InvalidOperationException("This is a simulated unhandled UI-thread failure.");
}
