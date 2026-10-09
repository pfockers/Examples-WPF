using System.Collections.ObjectModel;
using System.Windows;

namespace WpfAdvancedTraining.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private bool _isDarkTheme;
    private TopicNavigationItemViewModel? _selectedTopic;

    public MainViewModel()
    {
        Session = new TrainingSession();
        Session.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(TrainingSession.StatusMessage))
            {
                OnPropertyChanged(nameof(StatusMessage));
            }
        };
        Topics =
        [
            new("1. DataTemplates", "TemplatesTopic", new Topic01TemplatesViewModel(Session)),
            new("2. Styles & ControlTemplates", "StylesTopic", new Topic02StylesViewModel(Session)),
            new("3. DataTriggers", "DataTriggersTopic", new Topic03DataTriggersViewModel(Session)),
            new("4. Attached Properties", "AttachedPropertiesTopic", new Topic04AttachedPropertiesViewModel(Session)),
            new("5. Commands with Parameters", "CommandsTopic", new Topic05CommandsViewModel(Session)),
            new("6. CollectionView", "CollectionViewTopic", new Topic06CollectionViewViewModel(Session)),
            new("7. Value Converters", "ConvertersTopic", new Topic07ConvertersViewModel(Session)),
            new("8. Async Loading", "AsyncLoadingTopic", new Topic08AsyncLoadingViewModel(Session)),
            new("9. Validation", "ValidationTopic", new Topic09ValidationViewModel(Session)),
            new("10. View Navigation", "NavigationTopic", new Topic10NavigationViewModel(Session)),
            new("11. Behaviors", "BehaviorsTopic", new Topic11BehaviorsViewModel(Session)),
            new("12. Virtualization", "VirtualizationTopic", new Topic12VirtualizationViewModel(Session)),
            new("13. Dependency Properties", "DependencyPropertiesTopic", new Topic13DependencyPropertiesViewModel(Session)),
            new("14. Resource Dictionaries", "ResourceDictionariesTopic", new Topic14ResourceDictionariesViewModel(Session))
        ];
        SelectedTopic = Topics[0];
    }

    public TrainingSession Session { get; }

    public ObservableCollection<TopicNavigationItemViewModel> Topics { get; }

    public TopicNavigationItemViewModel? SelectedTopic
    {
        get => _selectedTopic;
        set => SetProperty(ref _selectedTopic, value);
    }

    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set
        {
            if (SetProperty(ref _isDarkTheme, value))
            {
                SetTheme(value);
            }
        }
    }

    public string StatusMessage => Session.StatusMessage;

    private static void SetTheme(bool isDark)
    {
        var dictionaries = Application.Current.Resources.MergedDictionaries;
        var themeDictionary = dictionaries.FirstOrDefault(dictionary =>
            dictionary.Source?.OriginalString.EndsWith("Theme.xaml", StringComparison.OrdinalIgnoreCase) == true);

        if (themeDictionary is not null)
        {
            themeDictionary.Source = new Uri(
                isDark ? "Resources/DarkTheme.xaml" : "Resources/LightTheme.xaml",
                UriKind.Relative);
        }
    }
}
