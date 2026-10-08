using System.ComponentModel;
using DependencyInjectionWpf.Services;

namespace DependencyInjectionWpf.ViewModels;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly IGreetingService _greetingService;

    public MainWindowViewModel(IGreetingService greetingService)
    {
        this._greetingService = greetingService;
    }

    public string Name
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
        }
    } = "World";

    public string Greeting
    {
        get;
        private set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Greeting)));
        }
    } = "Enter a name and select Greet.";

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Greet() => this.Greeting = this._greetingService.CreateGreeting(Name);
}
