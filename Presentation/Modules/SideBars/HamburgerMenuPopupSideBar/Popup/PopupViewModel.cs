using System;
using System.Windows;
using System.Windows.Input;

using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using Unity;

using System.Collections.ObjectModel;
using System.Linq;

namespace Aksl.Modules.HamburgerMenuPopupSideBar.ViewModels
{
    public class PopupViewModel : BindableBase
    {
        #region Members
        #endregion

        #region Constructors
        public PopupViewModel()
        {
            AllLeafPopupSideBarItems = new();
        }
        #endregion

        #region Properties
        public ObservableCollection<PopupSideBarItemViewModel> AllLeafPopupSideBarItems { get; set; }

        private bool _allowsTransparency = true;
        public bool AllowsTransparency
        {
            get => _allowsTransparency;
            set => SetProperty<bool>(ref _allowsTransparency, value);
        }

        private bool _isOpen = false;
        public bool IsOpen
        {
            get => _isOpen;
            set => SetProperty<bool>(ref _isOpen, value);
        }

        private bool _staysOpen = true;
        public bool StaysOpen
        {
            get => _staysOpen;
            set => SetProperty<bool>(ref _staysOpen, value);
        }

        private System.Windows.Controls.Primitives.PlacementMode _placementMode = System.Windows.Controls.Primitives.PlacementMode.Right;
        public System.Windows.Controls.Primitives.PlacementMode Placement
        {
            get => _placementMode;
            set => SetProperty<System.Windows.Controls.Primitives.PlacementMode>(ref _placementMode, value);
        }

        private System.Windows.UIElement _placementTarget = null;
        public System.Windows.UIElement PlacementTarget
        {
            get => _placementTarget;
            set => SetProperty<System.Windows.UIElement>(ref _placementTarget, value);
        }

        private System.Windows.Controls.Primitives.PopupAnimation _popupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Slide;
        public System.Windows.Controls.Primitives.PopupAnimation PopupAnimation
        {
            get => _popupAnimation;
            set => SetProperty<System.Windows.Controls.Primitives.PopupAnimation>(ref _popupAnimation, value);
        }
        #endregion

        #region Clear Selected Event
        public void ClearSelectedPopupSideBarItems()
        {
           AllLeafPopupSideBarItems.Where(pi => pi.IsSelected).ToList().ForEach(psbi => 
           {
               psbi.IsSelected = false;
           });
        }
        #endregion
    }
}
