using System.Collections.Generic;


namespace Modules.Hive
{
    internal class AppLifecycleHandlerDescriptorComparer : IComparer<IAppLifecycleHandlerDescriptor>
    {
        public int Compare(IAppLifecycleHandlerDescriptor x, IAppLifecycleHandlerDescriptor y)
        {
            if (x.Layer > y.Layer)
            {
                return -1;
            }

            if (x.Layer < y.Layer)
            {
                return 1;
            }

            if (x.Order < y.Order)
            {
                return -1;
            }

            if (x.Order > y.Order)
            {
                return 1;
            }

            return 0;
        }
    }
}
