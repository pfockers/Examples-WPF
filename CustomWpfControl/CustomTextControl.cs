using System.Windows;
using System.Windows.Controls;

namespace CustomWpfControl;

public class CustomTextControl : Control
{
    static CustomTextControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(CustomTextControl),
            new FrameworkPropertyMetadata(typeof(CustomTextControl)));
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(CustomTextControl),
            new FrameworkPropertyMetadata(
                string.Empty,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}
