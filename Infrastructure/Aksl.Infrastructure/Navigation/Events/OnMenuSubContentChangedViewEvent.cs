
using Prism.Events;

namespace Aksl.Infrastructure.Events
{
    public class OnMenuSubContentChangedViewEvent : PubSubEvent<OnMenuSubContentChangedViewEvent>
    {

        #region Constructors
        public OnMenuSubContentChangedViewEvent()
        {
        }
        #endregion

        #region Properties
        public MenuItem CurrentMenuItem { get; set; }
        #endregion
    }
}