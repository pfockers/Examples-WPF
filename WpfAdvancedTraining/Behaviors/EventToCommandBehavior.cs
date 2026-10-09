using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfAdvancedTraining.Behaviors;

public static class EventToCommandBehavior
{
    public static readonly DependencyProperty DoubleClickCommandProperty =
        DependencyProperty.RegisterAttached(
            "DoubleClickCommand",
            typeof(ICommand),
            typeof(EventToCommandBehavior),
            new PropertyMetadata(null, OnDoubleClickCommandChanged));

    public static ICommand? GetDoubleClickCommand(DependencyObject element) =>
        (ICommand?)element.GetValue(DoubleClickCommandProperty);

    public static void SetDoubleClickCommand(DependencyObject element, ICommand? value) =>
        element.SetValue(DoubleClickCommandProperty, value);

    private static void OnDoubleClickCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not ListBox listBox)
        {
            return;
        }

        listBox.MouseDoubleClick -= OnMouseDoubleClick;
        if (e.NewValue is ICommand)
        {
            listBox.MouseDoubleClick += OnMouseDoubleClick;
        }
    }

    private static void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is not ListBox listBox)
        {
            return;
        }

        var parameter = (e.OriginalSource as FrameworkElement)?.DataContext ?? listBox.SelectedItem;
        var command = GetDoubleClickCommand(listBox);
        if (command?.CanExecute(parameter) == true)
        {
            command.Execute(parameter);
        }
    }
}
