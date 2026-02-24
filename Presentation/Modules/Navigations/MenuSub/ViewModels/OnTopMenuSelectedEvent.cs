
using Aksl.Infrastructure;
using Prism.Events;

namespace Aksl.Modules.MenuSub
{
    public class OnTopMenuSubSelectedEvent : PubSubEvent<OnTopMenuSubSelectedEvent>
    {

        #region Constructors
        public OnTopMenuSubSelectedEvent()
        {
        }
        #endregion

        #region Properties
        public MenuItem SelectedMenuItem { get; set; }
        #endregion
    }
}