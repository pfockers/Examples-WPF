using System.ComponentModel;
using ErrorHandlingLoggingWpf.Services;
using Microsoft.Extensions.Logging;

namespace ErrorHandlingLoggingWpf.ViewModels;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly IDemoOperationService _operationService;
    private readonly ILogger<MainWindowViewModel> _logger;
    private string _status = "Select an operation to see error handling and logging in action.";

    public MainWindowViewModel(
        IDemoOperationService operationService,
        ILogger<MainWindowViewModel> logger)
    {
        _operationService = operationService;
        _logger = logger;
    }

    public string Status
    {
        get => _status;
        private set
        {
            if (_status == value)
            {
                return;
            }

            _status = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void RunSuccessfulOperation()
    {
        try
        {
            Status = _operationService.RunSuccessfully();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "The demo operation failed.");
            Status = "The operation failed. Details were logged.";
        }
    }

    public void RunHandledFailure()
    {
        try
        {
            _operationService.ThrowHandledFailure();
        }
        catch (InvalidOperationException exception)
        {
            _logger.LogError(exception, "A recoverable demo operation failure was handled.");
            Status = "The expected failure was caught; details were logged and the application continues.";
        }
    }

    public void ThrowUnhandledFailure() => _operationService.ThrowUnhandledFailure();
}
