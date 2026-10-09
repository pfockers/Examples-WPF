using System.Collections.ObjectModel;
using System.Windows.Input;
using WpfAdvancedTraining.Models;

namespace WpfAdvancedTraining.ViewModels;

public sealed class Topic12VirtualizationViewModel(TrainingSession session) : TrainingTopicViewModel(session)
{
    public ObservableCollection<UserItemViewModel> Users => Session.Users;

    public ICommand AddSampleUsersCommand => Session.AddSampleUsersCommand;
}
