using System.Collections.Generic;
using System.Windows;

namespace Aksl.Modules.HamburgerMenuPopupSideBar
{
    public class PopupInformation
    {
        #region Constructors
        public PopupInformation()
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
        public bool AllowsTransparency { get; set; }
        public bool IsOpen { get; set; } = false;
        public System.Windows.Controls.Primitives.PlacementMode Placement { get; set; }
        public System.Windows.UIElement PlacementTarget { get; set; }
        public bool StaysOpen { get; set; }
        public System.Windows.Controls.Primitives.PopupAnimation PopupAnimation { get; set; }

     
        #endregion
    }
}
