using System.Windows;
using System.Windows.Input;

namespace WpfAdvancedTraining.Behaviors;

public static class FocusExtension
{
    public static readonly DependencyProperty IsFocusedProperty =
        DependencyProperty.RegisterAttached(
            "IsFocused",
            typeof(bool),
            typeof(FocusExtension),
            new PropertyMetadata(false, OnIsFocusedChanged));

    public static bool GetIsFocused(DependencyObject element) =>
        (bool)element.GetValue(IsFocusedProperty);

    public static void SetIsFocused(DependencyObject element, bool value) =>
        element.SetValue(IsFocusedProperty, value);

    private static void OnIsFocusedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not FrameworkElement element)
        {
            return;
        }

        element.Loaded -= OnElementLoaded;
        if (e.NewValue is true)
        {
            if (element.IsLoaded)
            {
                element.Focus();
                Keyboard.Focus(element);
            }
            else
            {
                element.Loaded += OnElementLoaded;
            }
        }
    }

    private static void OnElementLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement element)
        {
            element.Loaded -= OnElementLoaded;
            element.Focus();
            Keyboard.Focus(element);
        }
    }
}
