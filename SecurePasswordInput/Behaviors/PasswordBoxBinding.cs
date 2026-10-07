using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace SecurePasswordInput.Behaviors;

public static class PasswordBoxBinding
{
    public static readonly DependencyProperty SecurePasswordProperty =
        DependencyProperty.RegisterAttached(
            "SecurePassword",
            typeof(SecureString),
            typeof(PasswordBoxBinding),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnSecurePasswordChanged));

    private static readonly DependencyProperty IsUpdatingProperty =
        DependencyProperty.RegisterAttached(
            "IsUpdating",
            typeof(bool),
            typeof(PasswordBoxBinding),
            new PropertyMetadata(false));

    public static SecureString? GetSecurePassword(DependencyObject dependencyObject) =>
        (SecureString?)dependencyObject.GetValue(SecurePasswordProperty);

    public static void SetSecurePassword(DependencyObject dependencyObject, SecureString? value) =>
        dependencyObject.SetValue(SecurePasswordProperty, value);

    private static bool GetIsUpdating(DependencyObject dependencyObject) =>
        (bool)dependencyObject.GetValue(IsUpdatingProperty);

    private static void SetIsUpdating(DependencyObject dependencyObject, bool value) =>
        dependencyObject.SetValue(IsUpdatingProperty, value);

    private static void OnSecurePasswordChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not PasswordBox passwordBox)
        {
            return;
        }

        passwordBox.PasswordChanged -= OnPasswordChanged;
        passwordBox.PasswordChanged += OnPasswordChanged;

        if (GetIsUpdating(passwordBox) || e.NewValue is not SecureString securePassword)
        {
            return;
        }

        SetIsUpdating(passwordBox, true);
        try
        {
            passwordBox.Clear();
            passwordBox.Password = ToUnsecureString(securePassword);
        }
        finally
        {
            SetIsUpdating(passwordBox, false);
        }
    }

    private static void OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is not PasswordBox passwordBox || GetIsUpdating(passwordBox))
        {
            return;
        }

        SetIsUpdating(passwordBox, true);
        try
        {
            SetSecurePassword(passwordBox, passwordBox.SecurePassword.Copy());
        }
        finally
        {
            SetIsUpdating(passwordBox, false);
        }
    }

    private static string ToUnsecureString(SecureString securePassword)
    {
        nint pointer = nint.Zero;
        try
        {
            pointer = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(securePassword);
            return System.Runtime.InteropServices.Marshal.PtrToStringBSTR(pointer) ?? string.Empty;
        }
        finally
        {
            if (pointer != nint.Zero)
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(pointer);
            }
        }
    }
}
