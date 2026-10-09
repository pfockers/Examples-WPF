using System.Collections;
using System.ComponentModel;
using System.Net.Mail;

namespace WpfAdvancedTraining.ViewModels;

public sealed class UserEditorViewModel : ObservableObject, INotifyDataErrorInfo
{
    private readonly Dictionary<string, List<string>> _errors = new();
    private string _name = string.Empty;
    private string _email = string.Empty;

    public string Name
    {
        get => _name;
        set
        {
            if (SetProperty(ref _name, value))
            {
                ValidateName();
            }
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            if (SetProperty(ref _email, value))
            {
                ValidateEmail();
            }
        }
    }

    public bool HasErrors => _errors.Count > 0;

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? propertyName) =>
        propertyName is not null && _errors.TryGetValue(propertyName, out var errors)
            ? errors
            : Array.Empty<string>();

    private void ValidateName() =>
        SetError(nameof(Name), string.IsNullOrWhiteSpace(Name) ? "Name is required." : null);

    private void ValidateEmail()
    {
        var valid = MailAddress.TryCreate(Email, out var address) && address.Address == Email;
        SetError(nameof(Email), valid ? null : "Enter a valid email address.");
    }

    private void SetError(string propertyName, string? error)
    {
        if (error is null)
        {
            if (_errors.Remove(propertyName))
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                OnPropertyChanged(nameof(HasErrors));
            }

            return;
        }

        if (_errors.TryGetValue(propertyName, out var existing) && existing.Count == 1 && existing[0] == error)
        {
            return;
        }

        _errors[propertyName] = [error];
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        OnPropertyChanged(nameof(HasErrors));
    }
}
