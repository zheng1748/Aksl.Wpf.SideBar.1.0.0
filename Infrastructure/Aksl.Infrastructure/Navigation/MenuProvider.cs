using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;

namespace Aksl.Infrastructure
{
    public class MenuProvider : IMenuProvider
    {
        #region Members
        private bool _isInitialized = false;
        #endregion

        #region Constructors
        public MenuProvider()
        {
        }
        #endregion

        #region Properties
        public MenuItem Root { get; private set; }
        #endregion

        #region Build Menu Method
        public async Task<MenuItem> BuildMenuAsync(string menuFilePath)
        {
            using (Stream stream = await GetResourceStreamAsync(menuFilePath))
            {
                using (XmlReader xmlRdr = XmlReader.Create(stream))
                {
                    XElement rootElement = XDocument.Load(xmlRdr).Element("MenuItem");
                    Root = CreateMenuItemFromXml(rootElement);

                    if (rootElement.Elements("MenuItem").Count() > 0)
                    {
                        foreach (var child in rootElement.Elements("MenuItem"))
                        {
                            AddSubMenus(child, Root);
                        }
                    }
                }
            }

            return Root;
        }
        #endregion

        #region Add Sub Menu Methods
        private void AddSubMenus(XElement currentElement,MenuItem parent)
        {
            var newMenuItem = CreateMenuItemFromXml(currentElement);
            AddSubMenu(newMenuItem, parent);

            foreach (var child in currentElement.Elements("MenuItem"))
            {
                AddSubMenus(child, newMenuItem);
            }
        }

        private void AddSubMenu(MenuItem menuItem, MenuItem parentMenu)
        {
            parentMenu.SubMenus.Add(menuItem);
            menuItem.Parent = parentMenu;
        }

        private MenuItem CreateMenuItemFromXml(XElement element)
        {
            MenuItem menuItem = new()
            {
                Id = int.Parse(element.Attribute("Id")?.Value),
                Name = element.Attribute("Name")?.Value,
                Title = element.Attribute("Title")?.Value,
                NavigationName = element.Attribute("NavigationName")?.Value
            };

            if (element.Attribute("IconKind")?.Value is not null)
            {
                var iconKind = element.Attribute("IconKind").Value;
                menuItem.IconKind = iconKind;
            }

            if (element.Attribute("ItemType")?.Value is not null)
            {
                var type = element.Attribute("ItemType").Value;
                if (Enum.TryParse(type, out MenuItemType result))
                {
                    menuItem.ItemType = result;
                }
                else
                {
                    menuItem.ItemType = MenuItemType.Item;
                }
            }

            // var fontIcon = element.Attribute("FontIcon")?.Value;
            if (element.Attribute("Level")?.Value is not null)
            {
                menuItem.Level = int.Parse(element.Attribute("Level")?.Value);
            }
            //  var isSeparator = (element.Attribute("isSeparator")?.Value) == null ? false : bool.Parse(element.Attribute("isSeparator")?.Value);
            // var navServiceName = element.Attribute("navigationServiceName")?.Value;
            menuItem.ModuleName = element.Attribute("ModuleName")?.Value;
            menuItem.ViewName = element.Attribute("ViewName")?.Value;
           // menuItem.RequrePermissons = element.Attribute("RequrePermissons")?.Value;
            //  return new MenuItem() { Title = Title, Icon = fontIcon, Level = Level, ViewName = ViewName };
            if (element.Attribute("IsHome") is not null)
            {
                var isHome = element.Attribute("IsHome").Value;
                menuItem.IsHome = bool.Parse(isHome);
            }
            if (element.Attribute("IsSelectedOnInitialize") is not null)
            {
                var isSelectedOnInitialize = element.Attribute("IsSelectedOnInitialize").Value;
                menuItem.IsSelectedOnInitialize = bool.Parse(isSelectedOnInitialize);
            }
            if (element.Attribute("IsCacheable") is not null)
            {
                var isCacheable = element.Attribute("IsCacheable").Value;
                menuItem.IsCacheable = bool.Parse(isCacheable);
            }

            menuItem.WorkspaceRegionName = element.Attribute("WorkspaceRegionName")?.Value;
            menuItem.WorkspaceViewEventName = element.Attribute("WorkspaceViewEventName")?.Value;

            if (element.Attribute("IsNextNavigation") is not null)
            {
                var isNextNavigation = element.Attribute("IsNextNavigation").Value;
                menuItem.IsNextNavigation = bool.Parse(isNextNavigation);
            }
            if (element.Attribute("IsNexOnNotLeaf") is not null)
            {
                var isNexOnNotLeaf = element.Attribute("IsNexOnNotLeaf").Value;
                menuItem.IsNexOnNotLeaf = bool.Parse(isNexOnNotLeaf);
            }

            //if (element.Attribute("ElementName") is not null)
            //{
            //    menuItem.ElementName = element.Attribute("ElementName").Value;
            //}
            //menuItem.Mode = element.Attribute("Mode")?.Value;

            return menuItem;
        }
        #endregion

        #region Get ResourceStream Method
        private Task<Stream> GetResourceStreamAsync(string menuFilePath)
        {
            Uri uri = new(menuFilePath);
            StreamResourceInfo info = Application.GetResourceStream(uri);

            if (info is null || info.Stream is null)
            {
                throw new ApplicationException($"Missing resource file:{menuFilePath}");
            }

            return Task.FromResult(info.Stream);
        }
        #endregion
    }
}
