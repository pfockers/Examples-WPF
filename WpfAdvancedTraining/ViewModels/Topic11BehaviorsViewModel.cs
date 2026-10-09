using System.Collections.ObjectModel;
using System.Windows.Input;
using WpfAdvancedTraining.Models;

namespace WpfAdvancedTraining.ViewModels;

public sealed class Topic11BehaviorsViewModel(TrainingSession session) : TrainingTopicViewModel(session)
{
    public ObservableCollection<UserItemViewModel> Users => Session.Users;

    public ICommand OpenDetailsCommand => Session.OpenDetailsCommand;

    public object? CurrentView => Session.CurrentView;
}
