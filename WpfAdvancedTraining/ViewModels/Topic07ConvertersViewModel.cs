using System.Collections.ObjectModel;
using WpfAdvancedTraining.Models;

namespace WpfAdvancedTraining.ViewModels;

public sealed class Topic07ConvertersViewModel(TrainingSession session) : TrainingTopicViewModel(session)
{
    public ObservableCollection<UserItemViewModel> Users => Session.Users;

    public UserItemViewModel? SelectedUser
    {
        get => Session.SelectedUser;
        set => Session.SelectedUser = value;
    }
}
