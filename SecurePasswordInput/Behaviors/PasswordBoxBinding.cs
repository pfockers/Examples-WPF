using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SecurePasswordInput.Behaviors;

public static class PasswordBoxBinding
{
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(PasswordBoxBinding),
            new PropertyMetadata(false, OnIsEnabledChanged));

    public static readonly DependencyProperty SecurePasswordProperty =
        DependencyProperty.RegisterAttached(
            "SecurePassword",
            typeof(SecureString),
            typeof(PasswordBoxBinding),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static bool GetIsEnabled(DependencyObject dependencyObject) =>
        (bool)dependencyObject.GetValue(IsEnabledProperty);

    public static void SetIsEnabled(DependencyObject dependencyObject, bool value) =>
        dependencyObject.SetValue(IsEnabledProperty, value);

    public static SecureString? GetSecurePassword(DependencyObject dependencyObject) =>
        (SecureString?)dependencyObject.GetValue(SecurePasswordProperty);

    public static void SetSecurePassword(DependencyObject dependencyObject, SecureString? value) =>
        dependencyObject.SetCurrentValue(SecurePasswordProperty, value);

    private static void OnIsEnabledChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not PasswordBox passwordBox)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            passwordBox.PasswordChanged += OnPasswordChanged;
        }
        else
        {
            passwordBox.PasswordChanged -= OnPasswordChanged;
        }
    }

    private static void OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is not PasswordBox passwordBox)
        {
            return;
        }

        SetSecurePassword(passwordBox, passwordBox.SecurePassword.Copy());
        BindingOperations.GetBindingExpressionBase(passwordBox, SecurePasswordProperty)?.UpdateSource();
    }
}
