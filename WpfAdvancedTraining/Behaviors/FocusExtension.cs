using System.Windows;
using System.Windows.Input;

namespace WpfAdvancedTraining.Behaviors;

public static class FocusExtension
{
    public static readonly DependencyProperty FocusRequestProperty =
        DependencyProperty.RegisterAttached(
            "FocusRequest",
            typeof(int),
            typeof(FocusExtension),
            new PropertyMetadata(0, OnFocusRequestChanged));

    public static int GetFocusRequest(DependencyObject element) =>
        (int)element.GetValue(FocusRequestProperty);

    public static void SetFocusRequest(DependencyObject element, int value) =>
        element.SetValue(FocusRequestProperty, value);

    private static void OnFocusRequestChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not FrameworkElement element || Equals(e.OldValue, e.NewValue))
        {
            return;
        }

        element.Loaded -= OnElementLoaded;
        element.Unloaded -= OnElementUnloaded;
        if (element.IsLoaded)
        {
            Focus(element);
        }
        else
        {
            element.Loaded += OnElementLoaded;
        }
    }

    private static void OnElementLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement element)
        {
            element.Loaded -= OnElementLoaded;
            Focus(element);
        }
    }

    private static void OnElementUnloaded(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement element)
        {
            element.Loaded -= OnElementLoaded;
            element.Unloaded -= OnElementUnloaded;
        }
    }

    private static void Focus(FrameworkElement element)
    {
        element.Loaded -= OnElementLoaded;
        element.Unloaded -= OnElementUnloaded;
        element.Unloaded += OnElementUnloaded;
        element.Dispatcher.BeginInvoke(new Action(() =>
        {
            if (element.IsLoaded && element.Focusable && element.IsVisible && element.IsEnabled)
            {
                element.Focus();
                Keyboard.Focus(element);
            }
        }), System.Windows.Threading.DispatcherPriority.Input);
    }
}
