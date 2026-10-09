using System.Windows.Input;
using WpfAdvancedTraining.Commands;

namespace WpfAdvancedTraining.ViewModels;

public sealed class Topic02StylesViewModel(TrainingSession session) : TrainingTopicViewModel(session)
{
    private const string OriginalName = "Ava Chen";
    private const string OriginalEmail = "ava.chen@example.com";
    private string _name = OriginalName;
    private string _email = OriginalEmail;
    private string _formMessage = "Edit the sample values, then choose an action.";

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string FormMessage
    {
        get => _formMessage;
        private set => SetProperty(ref _formMessage, value);
    }

    public ICommand SaveCommand { get; } = new RelayCommand(parameter =>
    {
        if (parameter is Topic02StylesViewModel viewModel)
        {
            viewModel.FormMessage = string.IsNullOrWhiteSpace(viewModel.Name) || string.IsNullOrWhiteSpace(viewModel.Email)
                ? "Name and email are required."
                : $"Saved changes for {viewModel.Name.Trim()}.";
        }
    });

    public ICommand CancelCommand { get; } = new RelayCommand(parameter =>
    {
        if (parameter is Topic02StylesViewModel viewModel)
        {
            viewModel.Name = OriginalName;
            viewModel.Email = OriginalEmail;
            viewModel.FormMessage = "Changes cancelled; the sample values were restored.";
        }
    });

    public ICommand DeleteCommand { get; } = new RelayCommand(parameter =>
    {
        if (parameter is Topic02StylesViewModel viewModel)
        {
            viewModel.Name = string.Empty;
            viewModel.Email = string.Empty;
            viewModel.FormMessage = "The sample record was cleared (no real data is deleted).";
        }
    });
}
