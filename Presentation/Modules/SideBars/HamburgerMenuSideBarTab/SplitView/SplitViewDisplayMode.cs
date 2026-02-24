using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aksl.Modules.HamburgerMenuSideBarTab
{
    public enum SplitViewDisplayMode
    {
        [Description("覆盖")]
        Overlay,
        [Description("内嵌")]
        Inline,
        [Description("紧凑覆盖")]
        CompactOverlay,
        [Description("紧凑内嵌")]
        CompactInline
    }
}
