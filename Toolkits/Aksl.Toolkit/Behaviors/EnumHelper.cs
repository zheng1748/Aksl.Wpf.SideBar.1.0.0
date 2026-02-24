using System;
using System.Collections.Generic;
using System.Linq;

namespace Aksl.Toolkit
{
    public sealed class EnumHelper<T>
    {
        public static List<T> ToList()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }
    }
}
