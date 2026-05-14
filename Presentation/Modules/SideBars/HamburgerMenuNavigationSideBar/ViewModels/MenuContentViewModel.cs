using System.Collections.Generic;
using System.Collections.ObjectModel;

using Prism.Events;
using Prism.Mvvm;

using Aksl.Infrastructure;

namespace Aksl.Modules.HamburgerMenuNavigationSideBar.ViewModels
{
    public class MenuContentViewModel : BindableBase
    {
        #region Members
        private IEnumerable<MenuItem> _leafMenuItems;
        #endregion

        #region Constructors
        public MenuContentViewModel()
        {
        }
        public MenuContentViewModel(int groupIndex, IEnumerable<MenuItem> leafMenuItems)
        {
            GroupIndex = groupIndex;
            _leafMenuItems = leafMenuItems;

            MenuItems = new();
        }
        #endregion

        #region Properties
        public int GroupIndex { get; }

        public ObservableCollection<MenuItemViewModel> MenuItems { get;  set; }

        private MenuItemViewModel _selectedMenuItem;
        public MenuItemViewModel SelectedMenuItem
        {
            get => _selectedMenuItem;
            set
            {
                SetProperty(ref _selectedMenuItem, value);
                //var previewSelectedMenuItem = _selectedMenuItem;

                //if (SetProperty(ref _selectedMenuItem, value))
                //{
                //    if (previewSelectedMenuItem is not null && previewSelectedMenuItem.IsSelected)
                //    {
                //        previewSelectedMenuItem.IsSelected = false;
                //    }

                //    if (_selectedMenuItem is not null && !_selectedMenuItem.IsSelected)
                //    {
                //        _selectedMenuItem.IsSelected = true;
                //    }
                //}
            }
        }

        private bool _isPaneOpen = false;
        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set
            {
                if (SetProperty<bool>(ref _isPaneOpen, value))
                {
                    foreach (var mivm in MenuItems)
                    {
                        mivm.IsPaneOpen = value;
                    }
                }
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty<bool>(ref _isLoading, value);
        }
        #endregion

        #region Clear Selected MenuItem Method
        internal void ClearSelectedMenuItem()
        {
            if (SelectedMenuItem is not null)
            {
                //SelectedMenuItem.IsSelected = false; 
                SelectedMenuItem = null;
            }
        }

        internal void ResetSelectedMenuItem(MenuItemViewModel selectedMenuItemItem)
        {
            if (selectedMenuItemItem is not null)
            {
                SelectedMenuItem = selectedMenuItemItem;
            }
        }
        #endregion

        #region Create MenuItem ViewModel Method
        internal void CreateMenuItemViewModels()
        {
            int index = 0;

            IsLoading = true;

            foreach (var menuItem in _leafMenuItems)
            {
                MenuItemViewModel menuItemViewModel = new(GroupIndex, index++, menuItem);

                MenuItems.Add(menuItemViewModel);
            }

            IsLoading = false;
        }
        #endregion
    }
}
