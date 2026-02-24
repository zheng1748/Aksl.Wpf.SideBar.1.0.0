using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aksl.Infrastructure
{
    public class MenuService : IMenuService
    {
        #region Members
        private readonly IEnumerable<string> _menuFilePaths;
        private readonly MenuProvider _menuProvider;
        private readonly List<MenuItem> _rootMenus;
        #endregion

        #region Constructors
        public MenuService(IEnumerable<string> menuFilePaths)
        {
            _menuFilePaths = menuFilePaths ?? throw new ArgumentNullException(nameof(menuFilePaths));

            _menuProvider = new();
            _rootMenus = new();
        }
        #endregion

        #region Properties
        public IEnumerable<MenuItem> Roots => _rootMenus;
        #endregion

        #region IMenuService
        public async Task CreateMenusAsync()
        {
            foreach (var menuFilePath in _menuFilePaths)
            {
                var rootMenu = await _menuProvider.BuildMenuAsync(menuFilePath);
                _rootMenus.Add(rootMenu);
            }
        }

        public  Task<MenuItem> GetMenuAsync(string menuName)
        {
            var rootMenuItem = _rootMenus.FirstOrDefault(r => !string.IsNullOrEmpty(r.Name) && r.Name.Equals(menuName, StringComparison.InvariantCultureIgnoreCase));

            return Task.FromResult(rootMenuItem);
            //List<MenuItem> rootMenus=new();

            //foreach (var menuFilePath in _menuFilePaths)
            //{
            //    var rootMenu = await _menuProvider.BuildMenuAsync(menuFilePath);

            //    rootMenus.Add(rootMenu);
            //}

            //var rootMenuItem = rootMenus.FirstOrDefault(r => !string.IsNullOrEmpty(r.Name) && r.Name.Equals(menuName, StringComparison.InvariantCultureIgnoreCase));
            //return rootMenuItem;
        }
        #endregion
    }
}


