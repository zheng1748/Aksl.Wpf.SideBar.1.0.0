
using Prism.Events;

namespace Aksl.Modules.TabBar
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