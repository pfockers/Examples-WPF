namespace WpfAdvancedTraining.ViewModels;

public sealed class TopicNavigationItemViewModel(string title, string templateKey)
{
    public string Title { get; } = title;

    public string TemplateKey { get; } = templateKey;
}
