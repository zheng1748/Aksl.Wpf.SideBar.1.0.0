using System;
using System.ComponentModel;

namespace Aksl.Modules.AirCompresser
{
    public enum CompressStatus
    {
        [Description("正常")]
        Normal,
        [Description("异常")]
        Error
    }
}
