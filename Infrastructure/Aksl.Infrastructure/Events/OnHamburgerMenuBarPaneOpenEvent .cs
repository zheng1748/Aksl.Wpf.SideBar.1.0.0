
using Prism.Events;

namespace Aksl.Infrastructure.Events
{
    public class OnHamburgerMenuBarPaneOpenEvent : PubSubEvent<OnHamburgerMenuBarPaneOpenEvent>
    {
        #region Constructors
        public OnHamburgerMenuBarPaneOpenEvent()
        {
        }
        #endregion

        #region Properties
        public bool IsPaneOpen { get; set; }
        #endregion
    }
}