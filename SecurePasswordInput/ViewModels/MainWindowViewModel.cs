using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Input;

namespace SecurePasswordInput.ViewModels;

public sealed class MainWindowViewModel : INotifyPropertyChanged, IDisposable
{
    private SecureString? _password;
    private SecureString? _confirmationPassword;
    private string _statusMessage = "Geben Sie ein Passwort mit mindestens 8 Zeichen ein.";

    public event PropertyChangedEventHandler? PropertyChanged;

    public SecureString? Password
    {
        get => _password;
        set
        {
            ReplaceSecureString(ref _password, value);
            OnPropertyChanged(nameof(Password));
            OnPropertyChanged(nameof(CanSubmit));
            ((RelayCommand)SubmitCommand).RaiseCanExecuteChanged();
        }
    }

    public SecureString? ConfirmationPassword
    {
        get => _confirmationPassword;
        set
        {
            ReplaceSecureString(ref _confirmationPassword, value);
            OnPropertyChanged(nameof(ConfirmationPassword));
            OnPropertyChanged(nameof(CanSubmit));
            ((RelayCommand)SubmitCommand).RaiseCanExecuteChanged();
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set
        {
            if (_statusMessage == value)
            {
                return;
            }

            _statusMessage = value;
            OnPropertyChanged(nameof(StatusMessage));
        }
    }

    public bool CanSubmit =>
        _password is { Length: >= 8 } &&
        _confirmationPassword is { Length: > 0 };

    public ICommand SubmitCommand { get; }

    public MainWindowViewModel()
    {
        SubmitCommand = new RelayCommand(Submit, () => CanSubmit);
    }

    private void Submit()
    {
        if (_password is null || _confirmationPassword is null)
        {
            return;
        }

        if (!AreEqual(_password, _confirmationPassword))
        {
            StatusMessage = "Die Passwörter stimmen nicht überein.";
            return;
        }

        StatusMessage = "Passwort erfolgreich validiert.";
        ClearPasswords();
    }

    private void ClearPasswords()
    {
        ReplaceSecureString(ref _password, null);
        ReplaceSecureString(ref _confirmationPassword, null);
        OnPropertyChanged(nameof(Password));
        OnPropertyChanged(nameof(ConfirmationPassword));
        OnPropertyChanged(nameof(CanSubmit));
        ((RelayCommand)SubmitCommand).RaiseCanExecuteChanged();
    }

    public void Dispose()
    {
        ReplaceSecureString(ref _password, null);
        ReplaceSecureString(ref _confirmationPassword, null);
        GC.SuppressFinalize(this);
    }

    private static void ReplaceSecureString(ref SecureString? target, SecureString? value)
    {
        target?.Dispose();
        target = value;
    }

    private static bool AreEqual(SecureString first, SecureString second)
    {
        if (first.Length != second.Length)
        {
            return false;
        }

        nint firstPointer = nint.Zero;
        nint secondPointer = nint.Zero;
        try
        {
            firstPointer = Marshal.SecureStringToBSTR(first);
            secondPointer = Marshal.SecureStringToBSTR(second);

            var result = 0;
            for (var index = 0; index < first.Length; index++)
            {
                result |= Marshal.ReadInt16(firstPointer, index * sizeof(char)) ^
                          Marshal.ReadInt16(secondPointer, index * sizeof(char));
            }

            return result == 0;
        }
        finally
        {
            if (firstPointer != nint.Zero)
            {
                Marshal.ZeroFreeBSTR(firstPointer);
            }

            if (secondPointer != nint.Zero)
            {
                Marshal.ZeroFreeBSTR(secondPointer);
            }
        }
    }

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public sealed class RelayCommand(Action execute, Func<bool>? canExecute = null) : ICommand
{
    private readonly Action _execute = execute;
    private readonly Func<bool> _canExecute = canExecute ?? (() => true);

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => _canExecute();

    public void Execute(object? parameter) => _execute();

    public void RaiseCanExecuteChanged() =>
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
