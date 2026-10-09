using System.Windows;
using System.Windows.Controls;

namespace WpfAdvancedTraining.Controls;

public class UserCardControl : Control
{
    static UserCardControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(UserCardControl),
            new FrameworkPropertyMetadata(typeof(UserCardControl)));
    }

    public static readonly DependencyProperty UserNameProperty =
        DependencyProperty.Register(
            nameof(UserName),
            typeof(string),
            typeof(UserCardControl),
            new FrameworkPropertyMetadata(string.Empty));

    public static readonly DependencyProperty RoleProperty =
        DependencyProperty.Register(
            nameof(Role),
            typeof(string),
            typeof(UserCardControl),
            new FrameworkPropertyMetadata(string.Empty));

    public static readonly DependencyProperty IsLockedProperty =
        DependencyProperty.Register(
            nameof(IsLocked),
            typeof(bool),
            typeof(UserCardControl),
            new FrameworkPropertyMetadata(false));

    public string UserName
    {
        get => (string)GetValue(UserNameProperty);
        set => SetValue(UserNameProperty, value);
    }

    public string Role
    {
        get => (string)GetValue(RoleProperty);
        set => SetValue(RoleProperty, value);
    }

    public bool IsLocked
    {
        get => (bool)GetValue(IsLockedProperty);
        set => SetValue(IsLockedProperty, value);
    }
}
