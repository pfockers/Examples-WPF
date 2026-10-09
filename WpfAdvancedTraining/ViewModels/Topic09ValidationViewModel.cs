using System.Windows.Input;

namespace WpfAdvancedTraining.ViewModels;

public sealed class Topic09ValidationViewModel(TrainingSession session) : TrainingTopicViewModel(session)
{
    public UserEditorViewModel Editor => Session.Editor;

    public ICommand AddValidatedUserCommand => Session.AddValidatedUserCommand;
}
