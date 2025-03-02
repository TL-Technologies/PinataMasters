using System;
using System.Collections.Generic;
using System.Linq;


public static class BaseExtension
{
    public static Version ToVersion(this string v)
    {
        return new Version(v);
    }


    public static IEnumerable<T> GetEnumValues<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }
}
