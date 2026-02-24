using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aksl.Infrastructure
{
    public interface IMenuService
    {
        #region Properties
        IEnumerable<MenuItem> Roots { get; }
        #endregion

        #region Methods
        Task CreateMenusAsync();

        Task<MenuItem> GetMenuAsync(string menuName);
        #endregion
    }
}
