using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using Prism;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity;
using Unity;

using Aksl.Toolkit.Services;
using Aksl.Infrastructure;

namespace Aksl.Modules.TabBar.ViewModels
{
    public class TabBarHubViewModel : BindableBase, INavigationAware
    {
        #region Members
        private readonly IUnityContainer _container;
        private readonly IDialogViewService _dialogViewService;
        private readonly IMenuService _menuService;
        #endregion

        #region Constructors
        public TabBarHubViewModel()
        {
            _container = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IUnityContainer>();
            _dialogViewService = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IDialogViewService>();

            _menuService = _container.Resolve<IMenuService>();

            TabViewModel = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<TabViewModel>();
        }
        #endregion

        #region Properties
        public TabViewModel TabViewModel { get; set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty<bool>(ref _isLoading, value);
        }
        #endregion

        #region Create TabViewModel Method
        private async Task CreateTabViewModelAsync(MenuItem currentMenuItem)
        {
            IsLoading = true;

            try
            {
                IEnumerable<MenuItem> subMenus = default;
                List<MenuItem> allLeafMenuItems = new();

                if (!string.IsNullOrEmpty(currentMenuItem.NavigationName))
                {
                    var parentMenuItem = await _menuService.GetMenuAsync(currentMenuItem.NavigationName);
                    subMenus = parentMenuItem.SubMenus;
                }

                if (string.IsNullOrEmpty(currentMenuItem.NavigationName) && HasSubMenu(currentMenuItem) && IsExistsViewInSubMenu(currentMenuItem))
                {
                    subMenus = currentMenuItem.SubMenus.Where(sm => !string.IsNullOrEmpty(sm.ViewName)).ToList();
                }

                bool HasSubMenu(MenuItem mi) => (mi is not null) && mi.SubMenus.Any();

                bool IsExistsViewInSubMenu(MenuItem mi) => (mi is not null) && mi.SubMenus.Any(sm => !string.IsNullOrEmpty(sm.ViewName));

                if (subMenus is not null)
                {
                    await GetLeafMenuItemsAsync();
                    AddTabViewModels();
                }

                async Task GetLeafMenuItemsAsync()
                {
                    foreach (var smi in subMenus)
                    {
                        var leafMenuItems = await GetAllLeafMenuItemsAsync(smi);
                        allLeafMenuItems.AddRange(leafMenuItems);
                    }
                }

                void AddTabViewModels()
                {
                    foreach (var mi in allLeafMenuItems)
                    {
                        TabInformation tabInformation = new()
                        {
                            Name = mi.Name,
                            Title = mi.Title,
                            IconKind = mi.IconKind,
                            ViewName = mi.ViewName,
                            CloseTabButtonVisibility = Visibility.Collapsed
                        };

                        TabViewModel.Add(tabInformation);
                    }

                    TabViewModel.SetFirstActiveTabItem();
                }
            }
            catch (Exception ex)
            {
                await _dialogViewService.AlertAsync(message: $"Unable to create tab view : \"{ex.Message}\"", title: "Error: Create TabView");
            }
            finally
            {
                if (IsLoading)
                {
                    IsLoading = false;
                }
            }
        }
        #endregion

        #region Get All LeafMenuItems Method
        private async Task<IEnumerable<MenuItem>> GetAllLeafMenuItemsAsync(MenuItem menuItem)
        {
            List<MenuItem> leafMenuItems = new();

            //if (HasSubMenu(menuItem))
            //{
            //    foreach (var smi in menuItem.SubMenus)
            //    {
            //        await RecursiveSubMenuItem(smi);
            //    }
            //}
            //else if (HasNavigationName(menuItem) && IsLeaf(menuItem))
            //{
            //    var root = await _menuService.GetMenuAsync(menuItem.NavigationName);
            //    foreach (var smi in root.SubMenus)
            //    {
            //        await RecursiveSubMenuItem(smi);
            //    }
            //}
            //else
            //{
            //    await RecursiveSubMenuItem(menuItem);
            //}

            await RecursiveSubMenuItem(menuItem);

            async Task RecursiveSubMenuItem(MenuItem currentMenuItem)
            {
                //if (!AnyEqualsMenuItem(leafMenuItems, currentMenuItem) && IsLeaf(currentMenuItem) && HasTitle(currentMenuItem))
                //{
                //    leafMenuItems.Add(currentMenuItem);
                //}

                var isAddOnLeaf = IsLeaf(currentMenuItem) && (!HasNavigationName(currentMenuItem) || (HasNavigationName(currentMenuItem) && !IsNextNavigation(currentMenuItem)));
                var isAddOnNotLeaf = !IsLeaf(currentMenuItem) && !IsNexOnNotLeaf(currentMenuItem);
                //  if (!AnyEqualsMenuItem(leafMenuItems, currentMenuItem) && IsLeaf(currentMenuItem) && !HasNavigationName(currentMenuItem) && HasTitle(currentMenuItem))
                if (!AnyEqualsMenuItem(leafMenuItems, currentMenuItem) && HasTitle(currentMenuItem) && (isAddOnLeaf || isAddOnNotLeaf))
                {
                    leafMenuItems.Add(currentMenuItem);
                }

                // if (HasNavigationName(currentMenuItem) && IsLeaf(currentMenuItem))
                // if (HasNavigationName(currentMenuItem) && IsNextNavigation(currentMenuItem) && IsLeaf(currentMenuItem))
                if (HasNavigationName(currentMenuItem) && IsNextNavigation(currentMenuItem))
                {
                    currentMenuItem = await _menuService.GetMenuAsync(currentMenuItem.NavigationName);
                }

                //if (HasSubMenu(currentMenuItem))
                if (HasSubMenu(currentMenuItem) && IsNexOnNotLeaf(currentMenuItem))
                {
                    foreach (var smi in currentMenuItem.SubMenus)
                    {
                        await RecursiveSubMenuItem(smi);
                    }
                }
            }

            bool HasSubMenu(MenuItem mi) => (mi is not null) && mi.SubMenus.Any();

            bool IsLeaf(MenuItem mi) => (mi is not null) && mi.SubMenus.Count <= 0;

            bool HasTitle(MenuItem mi) => (mi is not null) && !string.IsNullOrEmpty(mi.Title);

            bool IsNextNavigation(MenuItem mi) => (mi is not null) && mi.IsNextNavigation;

            bool HasNavigationName(MenuItem mi) => (mi is not null) && !string.IsNullOrEmpty(mi.NavigationName);

            bool IsNexOnNotLeaf(MenuItem mi) => (mi is not null) && mi.IsNexOnNotLeaf;

            return leafMenuItems;
        }
        #endregion

        #region Contain Methods
        private bool AnyEqualsMenuItem(IEnumerable<MenuItem> menuItems, MenuItem menuItem)
        {
            var isEquals = menuItems.Any(mi => IsEqualsNameOrTitle(mi.Title, menuItem.Title) || IsEqualsNameOrTitle(mi.Name, menuItem.Name));

            return isEquals;
        }

        private bool IsEqualsNameOrTitle(string nameOrTitle, string otherNameOrTitle)
        {
            if (string.IsNullOrEmpty(nameOrTitle) || string.IsNullOrEmpty(otherNameOrTitle))
            {
                return false;
            }

            var isEquals = (!string.IsNullOrEmpty(nameOrTitle) && otherNameOrTitle.Equals(nameOrTitle, StringComparison.InvariantCultureIgnoreCase)) ||
                           (!string.IsNullOrEmpty(nameOrTitle) && otherNameOrTitle.Equals(nameOrTitle, StringComparison.InvariantCultureIgnoreCase));

            return isEquals;
        }
        #endregion

        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var parameters = navigationContext.Parameters;
            if (parameters.TryGetValue("CurrentMenuItem", out MenuItem currentMenuItem))
            {
                CreateTabViewModelAsync(currentMenuItem).GetAwaiter().GetResult();
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
        #endregion
    }
}
