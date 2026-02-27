using Aksl.Modules.HamburgerMenuPopupSideBar.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aksl.Modules.HamburgerMenuPopupSideBar.ViewModels
{
    public class PopupViewModelPair
    {
        public PopupViewModel ThisPopupViewModel { get; set; }
        public PopupSideBarItemViewModel SelectedPopupSideBarItem { get; set; }
    }
}
