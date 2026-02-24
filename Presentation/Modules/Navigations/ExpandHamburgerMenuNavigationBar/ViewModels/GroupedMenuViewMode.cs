using System.Collections.Generic;

using Prism.Events;
using Prism.Mvvm;

using Aksl.Infrastructure;

namespace Aksl.Modules.ExpandHamburgerMenuNavigationBar.ViewModels
{
    public class GroupedMenuViewModel : BindableBase
    {
        #region Members
        private readonly IEventAggregator _eventAggregator;
        private readonly MenuItem _headerMenuItem;
        private IEnumerable<MenuItem> _leafMenuItems;
        #endregion

        #region Constructors
        public GroupedMenuViewModel(IEventAggregator eventAggregator, int groupIndex, MenuItem headerMenuItem, IEnumerable<MenuItem> leafMenuItems)
        {
            _eventAggregator = eventAggregator;
            GroupIndex = groupIndex;
            _leafMenuItems = leafMenuItems;
            _headerMenuItem = headerMenuItem;

            CreateMenuContentViewModels();
        }
        #endregion

        #region Properties
        public int GroupIndex { get; }

        public string HeaderTitle => _headerMenuItem.Title;

        public MenuContentViewModel MenuContent { get; private set; }

        public MenuItemViewModel SelectedMenuItem { get; private set; }

        public bool IsMoreCount => MenuContent.MenuItems.Count <= 5;

        public bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
        #endregion

        #region Create MenuContent ViewModel Method
        internal void CreateMenuContentViewModels()
        {
            IsLoading = true;

            MenuContentViewModel menuContentViewModel = new(_eventAggregator, GroupIndex, _leafMenuItems);
            AddPropertyChanged();

            void AddPropertyChanged()
            {
                menuContentViewModel.PropertyChanged += (sender, e) =>
                {
                    if (sender is MenuContentViewModel mcvm)
                    {
                        if (e.PropertyName == nameof(MenuContentViewModel.IsLoading) && !mcvm.IsLoading)
                        {
                            IsLoading = false;
                        }

                        if (e.PropertyName == nameof(MenuContentViewModel.SelectedMenuItem))
                        {
                            SelectedMenuItem = mcvm.SelectedMenuItem;
                            RaisePropertyChanged(nameof(MenuContent));
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
