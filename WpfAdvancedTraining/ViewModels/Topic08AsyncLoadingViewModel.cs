using System.Windows.Input;

namespace WpfAdvancedTraining.ViewModels;

public sealed class Topic08AsyncLoadingViewModel(TrainingSession session) : TrainingTopicViewModel(session)
{
    public ICommand LoadUsersCommand => Session.LoadUsersCommand;

    public bool IsLoading => Session.IsLoading;

    public string StatusMessage => Session.StatusMessage;
}
