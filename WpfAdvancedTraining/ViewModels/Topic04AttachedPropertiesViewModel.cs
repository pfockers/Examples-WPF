using System.Windows.Input;
using WpfAdvancedTraining.Commands;

namespace WpfAdvancedTraining.ViewModels;

public sealed class Topic04AttachedPropertiesViewModel(TrainingSession session) : TrainingTopicViewModel(session)
{
    private int _focusRequest;

    public int FocusRequest
    {
        get => _focusRequest;
        private set => SetProperty(ref _focusRequest, value);
    }

    public ICommand RequestFocusCommand { get; } = new RelayCommand(parameter =>
    {
        if (parameter is Topic04AttachedPropertiesViewModel viewModel)
        {
            viewModel.FocusRequest++;
        }
    });
}
