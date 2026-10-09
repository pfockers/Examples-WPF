using System.ComponentModel;

namespace WpfAdvancedTraining.ViewModels;

public abstract class TrainingTopicViewModel : ObservableObject
{
    protected TrainingTopicViewModel(TrainingSession session)
    {
        Session = session;
        Session.PropertyChanged += OnSessionPropertyChanged;
    }

    protected TrainingSession Session { get; }

    protected void OnSessionPropertyChanged(object? sender, PropertyChangedEventArgs e) =>
        OnPropertyChanged(e.PropertyName);
}
