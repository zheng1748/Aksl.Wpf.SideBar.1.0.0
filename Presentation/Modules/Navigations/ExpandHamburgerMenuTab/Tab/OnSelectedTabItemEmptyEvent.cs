
using Prism.Events;

namespace Aksl.Modules.ExpandHamburgerMenuTab
{
    public class OnSelectedTabItemEmptyEvent : PubSubEvent<OnSelectedTabItemEmptyEvent>
    {
        #region Constructors
        public OnSelectedTabItemEmptyEvent()
        {
            IsEmpty=true;
        }
        #endregion

        #region Properties
        public bool IsEmpty { get; set; }
        #endregion
    }
}