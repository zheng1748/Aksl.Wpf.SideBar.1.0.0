using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aksl.Infrastructure
{
    public interface IMenuProvider
    {
        MenuItem Root{ get; }

        Task<MenuItem> BuildMenuAsync(string menuFilePath);
    }
}
