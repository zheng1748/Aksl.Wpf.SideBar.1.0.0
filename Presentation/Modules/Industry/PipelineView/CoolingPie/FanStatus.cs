using System;
using System.ComponentModel;

namespace Aksl.Modules.Pipeline
{
    public enum FanStatus
    {
        [Description("转动")]
        Turn,
        [Description("暂停")]
        Pause
    }

    public enum TurnDirection
    {
        [Description("顺时针")]
        Clockwise,
        [Description("逆时针")]
        Counterclockwise
    }
}
