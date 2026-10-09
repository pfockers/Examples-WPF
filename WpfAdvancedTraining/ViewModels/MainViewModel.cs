using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using WpfAdvancedTraining.Commands;
using WpfAdvancedTraining.Models;

namespace WpfAdvancedTraining.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private string _searchText = string.Empty;
    private bool _isLoading;
    private UserItemViewModel? _selectedUser;
    private object? _currentView;
    private bool _isDarkTheme;
    private string _statusMessage = "Ready. Select a demo area to explore.";

    public MainViewModel()
    {
        Users =
        [
            new AdminUserViewModel("Ava Chen", "ava.chen@example.com"),
            new RegularUserViewModel("Noah Williams", "noah.williams@example.com"),
            new RegularUserViewModel("Mia Patel", "mia.patel@example.com", true),
            new AdminUserViewModel("Liam Garcia", "liam.garcia@example.com"),
            new RegularUserViewModel("Sofia Müller", "sofia.mueller@example.com"),
            new RegularUserViewModel("Ethan Brown", "ethan.brown@example.com", true),
            new AdminUserViewModel("Isabella Rossi", "isabella.rossi@example.com"),
            new RegularUserViewModel("Lucas Martin", "lucas.martin@example.com")
        ];

        UsersView = CollectionViewSource.GetDefaultView(Users);
        UsersView.Filter = FilterUser;
        UsersView.SortDescriptions.Add(new SortDescription(nameof(UserItemViewModel.Name), ListSortDirection.Ascending));
        UsersView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(UserItemViewModel.Role)));
        SelectedUser = Users[0];

        Topics =
        [
            new("1. DataTemplates", "TemplatesTopic"),
            new("2. Styles & ControlTemplates", "StylesTopic"),
            new("3. DataTriggers", "DataTriggersTopic"),
            new("4. Attached Properties", "AttachedPropertiesTopic"),
            new("5. Commands with Parameters", "CommandsTopic"),
            new("6. CollectionView", "CollectionViewTopic"),
            new("7. Value Converters", "ConvertersTopic"),
            new("8. Async Loading", "AsyncLoadingTopic"),
            new("9. Validation", "ValidationTopic"),
            new("10. View Navigation", "NavigationTopic"),
            new("11. Behaviors", "BehaviorsTopic"),
            new("12. Virtualization", "VirtualizationTopic"),
            new("13. Dependency Properties", "DependencyPropertiesTopic"),
            new("14. Resource Dictionaries", "ResourceDictionariesTopic")
        ];
        SelectedTopic = Topics[0];

        LockUserCommand = new RelayCommand(parameter => ToggleLock(parameter as UserItemViewModel));
        OpenDetailsCommand = new RelayCommand(parameter => ShowDetails(parameter as UserItemViewModel));
        AddSampleUsersCommand = new RelayCommand(_ => AddSampleUsers());
        ShowListCommand = new RelayCommand(_ => CurrentView = new DemoPageViewModel(
            "User list", "The list is shown through a ContentControl and a DataTemplate selected by view-model type."));
        LoadUsersCommand = new AsyncRelayCommand(LoadUsersAsync, () => !IsLoading);
        AddValidatedUserCommand = new RelayCommand(_ => AddValidatedUser(), _ => !Editor.HasErrors);
        Editor.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(UserEditorViewModel.HasErrors))
            {
                ((RelayCommand)AddValidatedUserCommand).RaiseCanExecuteChanged();
            }
        };

        CurrentView = new DemoPageViewModel(
            "User list", "The list is shown through a ContentControl and a DataTemplate selected by view-model type.");
    }

    public ObservableCollection<UserItemViewModel> Users { get; }

    public ICollectionView UsersView { get; }

    public ObservableCollection<TopicNavigationItemViewModel> Topics { get; }

    public TopicNavigationItemViewModel? SelectedTopic
    {
        get => _selectedTopic;
        set
        {
            if (SetProperty(ref _selectedTopic, value))
            {
                OnPropertyChanged(nameof(SelectedTopicView));
            }
        }
    }

    private TopicNavigationItemViewModel? _selectedTopic;

    public object? SelectedTopicView => SelectedTopic is null
        ? null
        : Application.Current.MainWindow?.FindResource(SelectedTopic.TemplateKey);

    public UserEditorViewModel Editor { get; } = new();

    public ICommand LockUserCommand { get; }

    public ICommand OpenDetailsCommand { get; }

    public ICommand AddSampleUsersCommand { get; }

    public ICommand ShowListCommand { get; }

    public AsyncRelayCommand LoadUsersCommand { get; }

    public ICommand AddValidatedUserCommand { get; }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                UsersView.Refresh();
            }
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            if (SetProperty(ref _isLoading, value))
            {
                LoadUsersCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public UserItemViewModel? SelectedUser
    {
        get => _selectedUser;
        set => SetProperty(ref _selectedUser, value);
    }

    public object? CurrentView
    {
        get => _currentView;
        private set => SetProperty(ref _currentView, value);
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

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    private bool FilterUser(object item)
    {
        if (item is not UserItemViewModel user)
        {
            return false;
        }

        return string.IsNullOrWhiteSpace(SearchText)
            || user.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
            || user.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
            || user.Role.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
    }

    private void ToggleLock(UserItemViewModel? user)
    {
        if (user is null)
        {
            return;
        }

        user.IsLocked = !user.IsLocked;
        StatusMessage = $"{user.Name} is now {(user.IsLocked ? "locked" : "active")}.";
    }

    private void ShowDetails(UserItemViewModel? user)
    {
        if (user is null)
        {
            return;
        }

        CurrentView = new DemoPageViewModel(
            $"Details: {user.Name}",
            $"Email: {user.Email} | Role: {user.Role} | Status: {(user.IsLocked ? "Locked" : "Active")}");
    }

    private async Task LoadUsersAsync()
    {
        IsLoading = true;
        StatusMessage = "Loading demo records...";

        try
        {
            await Task.Delay(1200);
            var nextNumber = Users.Count + 1;
            Users.Add(new RegularUserViewModel(
                $"New User {nextNumber}",
                $"new.user{nextNumber}@example.com"));
            StatusMessage = "A new demo user has been loaded.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void AddValidatedUser()
    {
        var name = Editor.Name.Trim();
        var email = Editor.Email.Trim();
        Users.Add(new RegularUserViewModel(name, email));
        Editor.Name = string.Empty;
        Editor.Email = string.Empty;
        StatusMessage = $"Added {name} after validation.";
    }

    private void AddSampleUsers()
    {
        var start = Users.Count + 1;
        for (var index = 0; index < 2000; index++)
        {
            var number = start + index;
            Users.Add(new RegularUserViewModel($"Sample User {number}", $"sample{number}@example.com"));
        }

        StatusMessage = "Added 2,000 users. Scroll the virtualized list to inspect container recycling.";
    }

    private static void SetTheme(bool isDark)
    {
        var dictionaries = Application.Current.Resources.MergedDictionaries;
        var themeDictionary = dictionaries.FirstOrDefault(dictionary =>
            dictionary.Source?.OriginalString.EndsWith("Theme.xaml", StringComparison.OrdinalIgnoreCase) == true);

        if (themeDictionary is null)
        {
            return;
        }

        themeDictionary.Source = new Uri(
            isDark ? "Resources/DarkTheme.xaml" : "Resources/LightTheme.xaml",
            UriKind.Relative);
    }
}
