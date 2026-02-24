
using Prism.Events;
using Prism.Mvvm;

namespace Aksl.Infrastructure.Events
{
    public class OnBuildIndustryWorkspaceViewEvent : PubSubEvent<OnBuildIndustryWorkspaceViewEvent>
    {
        #region Constructors
        public OnBuildIndustryWorkspaceViewEvent()
        {
        }
        #endregion

        #region Properties
        public MenuItem CurrentMenuItem { get; set; }
        #endregion
    }
}