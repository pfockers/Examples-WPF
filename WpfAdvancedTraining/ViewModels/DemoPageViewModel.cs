namespace WpfAdvancedTraining.ViewModels;

public sealed class DemoPageViewModel(string title, string description) : ObservableObject
{
    public string Title { get; } = title;

    public string Description { get; } = description;
}
