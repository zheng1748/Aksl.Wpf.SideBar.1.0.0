using Prism.Events;
using System;
using System.Collections;

namespace Aksl.Infrastructure.Events
{
    #region Eventbase
    public class OnBuildWorkspaceViewEvent<TPayload> : PubSubEvent<TPayload>
    {
        #region Constructors
        public OnBuildWorkspaceViewEvent()
        {
        }
        #endregion

        #region Properties
        public string Name { get; set; }

        public MenuItem CurrentMenuItem { get; set; }
        #endregion
    }

    public class OnBuildWorkspaceViewEventbase : PubSubEvent<OnBuildWorkspaceViewEventbase>, IEquatable<OnBuildWorkspaceViewEventbase>
    {
        #region Constructors
        public OnBuildWorkspaceViewEventbase()
        {
        }
        #endregion

        #region Properties
        public string Name { get; set; }

        public MenuItem CurrentMenuItem { get; set; }
        #endregion

        #region IEquatable Method
        public bool Equals(OnBuildWorkspaceViewEventbase other)
        {
            if (other is null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(other.Name))
            {
                return false;
            }


            return this.Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase);
        }
        #endregion
    }
    #endregion

    #region SideBar
    public class OnBuildHamburgerMenuSideBarWorkspaceViewEvent : OnBuildWorkspaceViewEventbase
    {
        #region Constructors
        public OnBuildHamburgerMenuSideBarWorkspaceViewEvent()
        {
            Name = typeof(OnBuildHamburgerMenuSideBarWorkspaceViewEvent).Name;
        }
        #endregion
    }

    public class OnBuildHamburgerMenuPopupSideBarWorkspaceViewEvent : OnBuildWorkspaceViewEventbase
    {
        #region Constructors
        public OnBuildHamburgerMenuPopupSideBarWorkspaceViewEvent()
        {
            Name = typeof(OnBuildHamburgerMenuPopupSideBarWorkspaceViewEvent).Name;
        }
        #endregion
    }

    public class OnBuildHamburgerMenuNavigationSideBarWorkspaceViewEvent : OnBuildWorkspaceViewEventbase
    {
        #region Constructors
        public OnBuildHamburgerMenuNavigationSideBarWorkspaceViewEvent()
        {
            Name = typeof(OnBuildHamburgerMenuNavigationSideBarWorkspaceViewEvent).Name;
        }
        #endregion
    }

    public class OnBuildHamburgerMenuTreeSideBarWorkspaceViewEvent : OnBuildWorkspaceViewEventbase
    {
        #region Constructors
        public OnBuildHamburgerMenuTreeSideBarWorkspaceViewEvent()
        {
            Name = typeof(OnBuildHamburgerMenuTreeSideBarWorkspaceViewEvent).Name;
        }
        #endregion
    }
    #endregion

    #region HamburgerMenu

    #endregion

    #region Expand HamburgerMenu

    #endregion

    #region Expand HamburgerMenu Tab

    #endregion

    #region Expand HamburgerMenuSub

    #endregion

    #region HamburgerMenuNavigationBar

    #endregion

    #region Expand HamburgerMenuNavigationBar

    #endregion

    #region MenuSub
    public class OnBuildIndustryMenuSubWorkspaceViewEvent : OnBuildWorkspaceViewEventbase
    {
        #region Constructors
        public OnBuildIndustryMenuSubWorkspaceViewEvent()
        {
            Name = typeof(OnBuildIndustryMenuSubWorkspaceViewEvent).Name;
        }
        #endregion
    }

    #endregion

    #region HamburgerMenuTreeBar

    #endregion

    #region Expand HamburgerMenuTreeBar

    #endregion
}