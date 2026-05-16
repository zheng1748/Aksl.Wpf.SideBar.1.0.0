using Aksl.Infrastructure;
using Aksl.Toolkit.Controls;
using Prism.Events;
using Prism.Mvvm;
using System;

namespace Aksl.Modules.HamburgerMenuNavigationSideBar.ViewModels
{
    public class MenuItemHeaderViewModel : BindableBase
    {
        #region Members
        private readonly MenuItem _menuItem;
        #endregion

        #region Constructors
        public MenuItemHeaderViewModel(MenuItem menuItem)
        {
            _menuItem = menuItem;

            HeaderTitle = _menuItem.Title;
        }
        #endregion

        #region Properties
        private string _headerTitle;
        public string HeaderTitle
        {
            get => _headerTitle;
            set => SetProperty<string>(ref _headerTitle, value);
        }

        public PackIconKind IconKind
        {
            get
            {
                PackIconKind kind = PackIconKind.None;

                _ = Enum.TryParse(_menuItem.IconKind, out kind);

                return kind;
            }
        }
        #endregion
    }
}
