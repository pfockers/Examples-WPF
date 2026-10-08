using System.Windows;
using System.Windows.Threading;
using ErrorHandlingLoggingWpf.Services;
using ErrorHandlingLoggingWpf.ViewModels;
using ErrorHandlingLoggingWpf.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace ErrorHandlingLoggingWpf;

using System.IO;

public partial class App
{
    private IHost? _host;
    private ILogger<App>? _logger;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var logDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ExamplesWpf",
            "ErrorHandlingLoggingWpf",
            "logs");
        var logFilePath = Path.Combine(logDirectory, "error-handling-.log");

        try
        {
            this._host = Host.CreateDefaultBuilder()
                .UseSerilog((_, configuration) => configuration
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .WriteTo.File(
                        logFilePath,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7)
                    .WriteTo.Debug())
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IDemoOperationService, DemoOperationService>();
                    services.AddTransient<MainWindowViewModel>();
                    services.AddTransient<MainWindow>();
                })
                .Build();

            await this._host.StartAsync();
            this._logger = this._host.Services.GetRequiredService<ILogger<App>>();
            this.DispatcherUnhandledException += this.OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += this.OnAppDomainUnhandledException;
            TaskScheduler.UnobservedTaskException += this.OnUnobservedTaskException;

            var mainWindow = this._host.Services.GetRequiredService<MainWindow>();
            this.MainWindow = mainWindow;
            mainWindow.Show();
            this._logger.LogInformation("Application started. Log file: {LogFilePath}", logFilePath);
        }
        catch (Exception exception)
        {
            this._logger?.LogCritical(exception, "Application startup failed.");
            MessageBox.Show(
                "The application could not start. See the log file for details.",
                "Startup error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            this.Shutdown(-1);
        }
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (this._host is not null)
        {
            await this._host.StopAsync();
            this._host.Dispose();
        }

        base.OnExit(e);
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        this._logger?.LogCritical(e.Exception, "Unhandled exception on the WPF dispatcher.");
        MessageBox.Show(
            "An unexpected error occurred. Details were written to the application log.",
            "Unexpected error",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }

    private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
        {
            this._logger?.LogCritical(exception, "Unhandled application-domain exception. Process terminating: {IsTerminating}", e.IsTerminating);
        }
        else
        {
            this._logger?.LogCritical("Unhandled non-exception object in the application domain. Process terminating: {IsTerminating}", e.IsTerminating);
        }
    }

    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        this._logger?.LogError(e.Exception, "An unobserved task exception occurred.");
        e.SetObserved();
    }
}
