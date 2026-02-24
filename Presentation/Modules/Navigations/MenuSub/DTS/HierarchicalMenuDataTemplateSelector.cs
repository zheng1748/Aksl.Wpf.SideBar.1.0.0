using System.Windows;
using System.Windows.Controls;

using Aksl.Modules.MenuSub.ViewModels;

namespace Aksl.Modules.MenuSub.DataTemplateSelectors
{
    public class HierarchicalMenuDataTemplateSelector : DataTemplateSelector
    {
        public HierarchicalMenuDataTemplateSelector() { }

        public DataTemplate CommandTemplate { set; get; }

        public DataTemplate SeparatorTemplate { set; get; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is HierarchicalMenuItemViewModel hmvm)
            {
                if (hmvm.IsSeparator)
                {
                    return SeparatorTemplate;
                }
                else
                {
                    return CommandTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
