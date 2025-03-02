using System;
using System.Collections.Generic;


namespace Modules.Hive.Editor.Pipeline
{
    internal class TypeHierarchyComparer : IComparer<Type>
    {
        public int Compare(Type x, Type y)
        {
            if (x == y)
            {
                return 0;
            }

            if (x.IsAssignableFrom(y))
            {
                return -1;
            }

            return 1;
        }
    }
}
