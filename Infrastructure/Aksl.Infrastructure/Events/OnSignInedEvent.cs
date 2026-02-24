using System;

using Prism.Events;

namespace Aksl.Infrastructure.Events
{
    public class OnSignInedEvent : PubSubEvent<OnSignInedEvent>
    {
        #region Constructors
        public OnSignInedEvent()
        { }
        #endregion

        #region Properties
        public string UserName { get; set; }

        public bool IsSuccessful { get; set; }
        #endregion
    }
}
