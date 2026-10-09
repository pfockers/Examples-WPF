using System.Windows.Input;

namespace WpfAdvancedTraining.ViewModels;

public sealed class Topic10NavigationViewModel(TrainingSession session) : TrainingTopicViewModel(session)
{
    public ICommand ShowListCommand => Session.ShowListCommand;

    public ICommand OpenDetailsCommand => Session.OpenDetailsCommand;

    public object? CurrentView => Session.CurrentView;

    public object? SelectedUser => Session.SelectedUser;
}
