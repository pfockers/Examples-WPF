using System.Windows;
using System.Windows.Controls;
using WpfAdvancedTraining.Models;

namespace WpfAdvancedTraining.Selectors;

public sealed class UserTemplateSelector : DataTemplateSelector
{
    public DataTemplate? AdminTemplate { get; set; }

    public DataTemplate? RegularTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container) =>
        item is AdminUserViewModel ? AdminTemplate : RegularTemplate;
}
