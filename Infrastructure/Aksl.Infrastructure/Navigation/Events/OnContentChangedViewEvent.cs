
using Prism.Events;

namespace Aksl.Infrastructure.Events
{
    public class OnContentChangedViewEvent : PubSubEvent<OnContentChangedViewEvent>
    {

        #region Constructors
        public OnContentChangedViewEvent()
        {
        }
        #endregion

        #region Properties
        public MenuItem CurrentMenuItem { get; set; }
        #endregion
    }
}