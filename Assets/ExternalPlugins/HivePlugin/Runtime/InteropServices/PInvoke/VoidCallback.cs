using System;
using System.Runtime.InteropServices;


namespace Modules.Hive.InteropServices.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VoidCallback
    {
        private readonly IntPtr ptr;
        private readonly Action<IntPtr> callback;


        internal VoidCallback(IntPtr ptr, Action<IntPtr> callback)
        {
            this.ptr = ptr;
            this.callback = callback;
        }


        /// <summary>
        /// <para>
        /// Creates a native callback struct for a managed static method.
        /// </para>
        /// <para>
        /// KEEP IN MIND that only static methods can be passed to argument <paramref name="staticMethod"/>!
        /// </para>
        /// </summary>
        /// <param name="staticMethod">A delegate to a static method.</param>
        /// <returns>A native callback struct for a managed static method.</returns>
        public static VoidCallback ForStaticMethod(Action<IntPtr> staticMethod)
        {
            return new VoidCallback(IntPtr.Zero, staticMethod);
        }
    }
}
