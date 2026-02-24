using System;
using System.ComponentModel;

namespace Aksl.Modules.CoolingTower
{
    public enum TowerStatus
    {
        [Description("正常")]
        Normal,
        [Description("异常")]
        Error
    }
}
