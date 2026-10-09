using System.Windows;
using System.Windows.Controls;
using WpfAdvancedTraining.ViewModels;

namespace WpfAdvancedTraining.Selectors;

public sealed class TopicContentTemplateSelector : DataTemplateSelector
{
    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (item is not TopicNavigationItemViewModel topic || container is not FrameworkElement element)
        {
            return null;
        }

        return element.TryFindResource(topic.TemplateKey) as DataTemplate;
    }
}
