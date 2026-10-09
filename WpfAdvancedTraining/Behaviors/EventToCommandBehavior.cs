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

        if (e.OldValue is not null)
        {
            listBox.MouseDoubleClick -= OnMouseDoubleClick;
            listBox.Loaded -= OnListBoxLoaded;
            listBox.Unloaded -= OnListBoxUnloaded;
        }

        if (e.NewValue is ICommand)
        {
            if (listBox.IsLoaded)
            {
                listBox.MouseDoubleClick += OnMouseDoubleClick;
            }

            listBox.Loaded += OnListBoxLoaded;
            listBox.Unloaded += OnListBoxUnloaded;
        }
    }

    private static void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is not ListBox listBox)
        {
            return;
        }

        var item = ItemsControl.ContainerFromElement(listBox, e.OriginalSource as DependencyObject) as ListBoxItem;
        if (item is null)
        {
            return;
        }

        var parameter = item.DataContext;
        var command = GetDoubleClickCommand(listBox);
        if (command?.CanExecute(parameter) == true)
        {
            command.Execute(parameter);
        }
    }

    private static void OnListBoxUnloaded(object sender, RoutedEventArgs e)
    {
        if (sender is ListBox listBox)
        {
            listBox.MouseDoubleClick -= OnMouseDoubleClick;
        }
    }

    private static void OnListBoxLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is ListBox listBox && GetDoubleClickCommand(listBox) is not null)
        {
            listBox.MouseDoubleClick -= OnMouseDoubleClick;
            listBox.MouseDoubleClick += OnMouseDoubleClick;
        }
    }
}
