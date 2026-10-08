namespace ErrorHandlingLoggingWpf.Services;

public interface IDemoOperationService
{
    string RunSuccessfully();

    void ThrowHandledFailure();

    void ThrowUnhandledFailure();
}
