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
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Aksl.Modules.ExpandHamburgerMenuSub
{
    public class ViewElementManager
    {
        #region Members
        #endregion

        #region Constructors
        public ViewElementManager()
        {
            StoreMenuItems = new();
        }
        #endregion

        #region Properties
        public List<MiniMenuItem> StoreMenuItems { get; }
        #endregion

        #region Methods
        public DependencyObject Add(MiniMenuItem miniMenuItem)
        {
            if (!IsExistsMenuItems(miniMenuItem.Name))
            {
                StoreMenuItems.Add(miniMenuItem);
            }

            return miniMenuItem?.ViewElement;
        }

        public void Remove(DependencyObject dependencyObject)
        {
            var menuItem = StoreMenuItems.FirstOrDefault(mi => mi.ViewElementType == dependencyObject.GetType());

            if (menuItem is not null)
            {
                StoreMenuItems.Remove(menuItem);
            }
        }

        public DependencyObject? GetStoreViewElementType(Type viewType)
        {
            var menuItem = StoreMenuItems.FirstOrDefault(mi=> mi.ViewElementType == viewType);

            return menuItem?.ViewElement;
        }
        #endregion

        #region Contain Methods
        private bool IsExistsMenuItems(string name)
        {
            var isExists = StoreMenuItems.Any(ti => IsEqualsNameOrTitle(ti.Name, name));

            return isExists;
        }

        private bool IsEqualsNameOrTitle(string name, string otherName)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(otherName))
            {
                return false;
            }

            var isEquals = (!string.IsNullOrEmpty(name) && name.Equals(otherName, StringComparison.InvariantCultureIgnoreCase)) ||
                           (!string.IsNullOrEmpty(otherName) && otherName.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            return isEquals;
        }
        #endregion
    }
}
