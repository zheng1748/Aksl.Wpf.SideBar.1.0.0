using System.Collections.Generic;
using System.Windows;

namespace Aksl.Modules.TabBar
{
    public class TabInformation
    {
        #region Constructors
        public TabInformation()
        {
        }
        #endregion

        #region Properties

        public string Name { get; set; }

        public string Title { get; set; }

        public string IconKind { get; set; }

        public string ViewName { get; set; }

        public DependencyObject ViewElement { get; set; }

        public string WorkspaceRegionName { get; set; }

        public string WorkspaceViewEventName { get; set; }

        public Visibility CloseTabButtonVisibility { get; set; } = Visibility.Visible;
        #endregion
    }
}
