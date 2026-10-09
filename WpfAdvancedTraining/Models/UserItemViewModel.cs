using System.ComponentModel;

namespace WpfAdvancedTraining.Models;

public abstract class UserItemViewModel : INotifyPropertyChanged
{
    private string _name;
    private string _email;
    private bool _isLocked;

    protected UserItemViewModel(string name, string email, bool isLocked = false)
    {
        _name = name;
        _email = email;
        _isLocked = isLocked;
    }

    public string Name
    {
        get => _name;
        set
        {
            if (_name == value)
            {
                return;
            }

            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            if (_email == value)
            {
                return;
            }

            _email = value;
            OnPropertyChanged(nameof(Email));
        }
    }

    public bool IsLocked
    {
        get => _isLocked;
        set
        {
            if (_isLocked == value)
            {
                return;
            }

            _isLocked = value;
            OnPropertyChanged(nameof(IsLocked));
        }
    }

    public abstract string Role { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public sealed class AdminUserViewModel(string name, string email, bool isLocked = false)
    : UserItemViewModel(name, email, isLocked)
{
    public override string Role => "Administrator";
}

public sealed class RegularUserViewModel(string name, string email, bool isLocked = false)
    : UserItemViewModel(name, email, isLocked)
{
    public override string Role => "User";
}
