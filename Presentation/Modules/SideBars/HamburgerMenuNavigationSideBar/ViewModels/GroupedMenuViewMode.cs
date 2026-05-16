using System.Collections.Generic;
using System.Collections.ObjectModel;

using Aksl.Infrastructure;

namespace Aksl.Modules.HamburgerMenuNavigationSideBar.ViewModels
{
    public class GroupedMenuViewModel : GroupedMenuViewModelBase
    {
        #region Members
        private readonly MenuItem _headerMenuItem;
        private IEnumerable<MenuItem> _leafMenuItems;
        #endregion

        #region Constructors
        public GroupedMenuViewModel(int groupIndex, MenuItem headerMenuItem, IEnumerable<MenuItem> leafMenuItems) : base()
        {
            GroupIndex = groupIndex;
            _leafMenuItems = leafMenuItems;
            _headerMenuItem = headerMenuItem;
            MenuItemHeader=new(headerMenuItem);
        }
        #endregion

        #region Properties
        public int GroupIndex { get; }

        public string HeaderTitle => _headerMenuItem.Title;
        public MenuItemHeaderViewModel MenuItemHeader { get; set; }
        public MenuContentViewModel MenuContent { get; private set; }

        private MenuItemViewModel _selectedMenuItem;
        public MenuItemViewModel SelectedMenuItem
        {
            get => _selectedMenuItem;
            set
            {
                SetProperty(ref _selectedMenuItem, value);
                //if (SetProperty(ref _selectedMenuItem, value))
                //{
                //    MenuContent.SelectedMenuItem = value;
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
                    MenuContent.IsPaneOpen = value;
                }
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
        #endregion

      
        #region Create MenuItem ViewModel Method
        internal void CreateMenuItemViewModels()
        {
            int index = 0;

            List<MenuItemViewModel> menuItems = new();

            foreach (var menuItem in _leafMenuItems)
            {
                MenuItemViewModel menuItemViewModel = new(GroupIndex, index++, menuItem);

                menuItems.Add(menuItemViewModel);
            }

            MenuContentViewModel menuContentViewModel = new()
            {
                MenuItems = new ObservableCollection<MenuItemViewModel>(menuItems)
            };

            AddPropertyChanged();
            void AddPropertyChanged()
            {
                menuContentViewModel.PropertyChanged += (sender, e) =>
                {
                    if (sender is MenuContentViewModel mcvm)
                    {
                        //if (e.PropertyName == nameof(MenuContentViewModel.IsLoading) && !mcvm.IsLoading)
                        //{
                        //    IsLoading = false;
                        //}

                        if (e.PropertyName == nameof(MenuContentViewModel.SelectedMenuItem))
                        {
                            //_selectedMenuItem = mcvm.SelectedMenuItem;
                            SelectedMenuItem = mcvm.SelectedMenuItem;
                            //  RaisePropertyChanged(nameof(MenuContent));
                        }
                    }
                };
            }

            MenuContent = menuContentViewModel;
        }
        #endregion

        #region Create MenuContent ViewModel Method
        internal void CreateMenuContentViewModels()
        {
            IsLoading = true;

            MenuContentViewModel menuContentViewModel = new(GroupIndex, _leafMenuItems);
            AddPropertyChanged();

            void AddPropertyChanged()
            {
                menuContentViewModel.PropertyChanged += (sender, e) =>
                {
                    if (sender is MenuContentViewModel mcvm)
                    {
                        //if (e.PropertyName == nameof(MenuContentViewModel.IsLoading) && !mcvm.IsLoading)
                        //{
                        //    IsLoading = false;
                        //}

                        if (e.PropertyName == nameof(MenuContentViewModel.SelectedMenuItem))
                        {
                            //_selectedMenuItem = mcvm.SelectedMenuItem;
                            SelectedMenuItem = mcvm.SelectedMenuItem;
                            //  RaisePropertyChanged(nameof(MenuContent));
                        }
                    }
                };
            }

            menuContentViewModel.CreateMenuItemViewModels();
            MenuContent = menuContentViewModel;

            IsLoading = false;
        }
        #endregion
    }
}
