

using Prism.Events;

namespace Aksl.Modules.HamburgerMenuTab
{
    public class OnActiveTabItemEvent : PubSubEvent<OnActiveTabItemEvent>
    {
        #region Constructors
        public OnActiveTabItemEvent()
        {
        }
        #endregion

        #region Properties
        public TabInformation SelectedTabItem { get; set; }
        #endregion
    }
}