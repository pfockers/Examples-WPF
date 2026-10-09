using System.Collections.ObjectModel;
using System.Windows.Input;
using WpfAdvancedTraining.Models;

namespace WpfAdvancedTraining.ViewModels;

public sealed class Topic05CommandsViewModel(TrainingSession session) : TrainingTopicViewModel(session)
{
    public ObservableCollection<UserItemViewModel> Users => Session.Users;

    public UserItemViewModel? SelectedUser
    {
        get => Session.SelectedUser;
        set => Session.SelectedUser = value;
    }

    public ICommand LockUserCommand => Session.LockUserCommand;
}
