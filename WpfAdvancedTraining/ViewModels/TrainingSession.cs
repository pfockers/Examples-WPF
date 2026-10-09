using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using WpfAdvancedTraining.Commands;
using WpfAdvancedTraining.Models;

namespace WpfAdvancedTraining.ViewModels;

public sealed class TrainingSession : ObservableObject
{
    private UserItemViewModel? _selectedUser;
    private bool _isLoading;
    private string _statusMessage = "Ready. Select a topic to explore.";
    private object? _currentView;

    public TrainingSession()
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

        SelectedUser = Users[0];
        UsersView = CollectionViewSource.GetDefaultView(Users);
        UsersView.SortDescriptions.Add(new SortDescription(nameof(UserItemViewModel.Name), ListSortDirection.Ascending));
        UsersView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(UserItemViewModel.Role)));
        LockUserCommand = new RelayCommand(parameter => ToggleLock(parameter as UserItemViewModel));
        OpenDetailsCommand = new RelayCommand(parameter => ShowDetails(parameter as UserItemViewModel));
        AddSampleUsersCommand = new RelayCommand(_ => AddSampleUsers());
        LoadUsersCommand = new AsyncRelayCommand(LoadUsersAsync, () => !IsLoading);
        Editor = new UserEditorViewModel();
        AddValidatedUserCommand = new RelayCommand(_ => AddValidatedUser(), _ => !Editor.HasErrors);
        Editor.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(UserEditorViewModel.HasErrors))
            {
                ((RelayCommand)AddValidatedUserCommand).RaiseCanExecuteChanged();
            }
        };
        ShowListCommand = new RelayCommand(_ => CurrentView = new DemoPageViewModel(
            "User list", "The list is shown through a ContentControl and a DataTemplate selected by view-model type."));
        CurrentView = new DemoPageViewModel(
            "User list", "The list is shown through a ContentControl and a DataTemplate selected by view-model type.");
    }

    public ObservableCollection<UserItemViewModel> Users { get; }

    public ICollectionView UsersView { get; }

    public UserEditorViewModel Editor { get; }

    public ICommand LockUserCommand { get; }

    public ICommand OpenDetailsCommand { get; }

    public ICommand AddSampleUsersCommand { get; }

    public ICommand AddValidatedUserCommand { get; }

    public ICommand ShowListCommand { get; }

    public AsyncRelayCommand LoadUsersCommand { get; }

    public UserItemViewModel? SelectedUser
    {
        get => _selectedUser;
        set => SetProperty(ref _selectedUser, value);
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

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public object? CurrentView
    {
        get => _currentView;
        private set => SetProperty(ref _currentView, value);
    }

    public ICollectionView CreateUsersView(string searchText) =>
        new ListCollectionView(Users)
        {
            Filter = item => item is UserItemViewModel user
                && (string.IsNullOrWhiteSpace(searchText)
                    || user.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                    || user.Email.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                    || user.Role.Contains(searchText, StringComparison.OrdinalIgnoreCase))
        };

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
        if (user is not null)
        {
            CurrentView = new DemoPageViewModel(
                $"Details: {user.Name}",
                $"Email: {user.Email} | Role: {user.Role} | Status: {(user.IsLocked ? "Locked" : "Active")}");
        }
    }

    private async Task LoadUsersAsync()
    {
        IsLoading = true;
        StatusMessage = "Loading demo records...";
        try
        {
            await Task.Delay(1200);
            var nextNumber = Users.Count + 1;
            Users.Add(new RegularUserViewModel($"New User {nextNumber}", $"new.user{nextNumber}@example.com"));
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
        Users.Add(new RegularUserViewModel(name, Editor.Email.Trim()));
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
}
