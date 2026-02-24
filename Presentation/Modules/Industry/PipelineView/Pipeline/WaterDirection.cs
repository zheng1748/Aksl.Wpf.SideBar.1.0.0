using System;
using System.ComponentModel;

namespace Aksl.Modules.Pipeline
{
    public enum WaterDirection
    {
        [Description("从西往东")]
        WestToEast,
        [Description("从东往西")]
        EastToWest,
        [Description("暂停")]
        Pause
    }
}
