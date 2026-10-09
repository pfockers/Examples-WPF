namespace WpfAdvancedTraining.ViewModels;

public sealed class TopicNavigationItemViewModel(string title, string templateKey, object viewModel)
{
    public string Title { get; } = title;

    public string TemplateKey { get; } = templateKey;

    public object ViewModel { get; } = viewModel;
}
