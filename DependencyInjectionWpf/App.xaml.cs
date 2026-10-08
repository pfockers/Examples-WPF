using System.Windows;
using DependencyInjectionWpf.Services;
using DependencyInjectionWpf.ViewModels;
using DependencyInjectionWpf.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DependencyInjectionWpf;

public partial class App : Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        this._host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<IGreetingService, GreetingService>();
                services.AddTransient<MainWindowViewModel>();
                services.AddTransient<MainWindow>();
            })
            .Build();

        await this._host.StartAsync();

        var mainWindow = this._host.Services.GetRequiredService<MainWindow>();
        this.MainWindow = mainWindow;
        mainWindow.Show();
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
}
