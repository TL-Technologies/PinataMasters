using System;
using System.Runtime.InteropServices;


namespace Modules.Hive.InteropServices.PInvoke
{
    public abstract class CallbackWrapper : IDisposable
    {
        private GCHandle handle;


        public CallbackWrapperOptions Options { get; }


        public bool IsTransient => (Options & CallbackWrapperOptions.Transient) != 0;


        public CallbackWrapper(CallbackWrapperOptions options)
        {
            Options = options;
        }


        protected static T FromIntPtr<T>(IntPtr ptr) where T : CallbackWrapper
        {
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            GCHandle handle = GCHandle.FromIntPtr(ptr);
            if (!handle.IsAllocated)
            {
                return null;
            }

            var wrapper = handle.Target as T;

            // Free the handle to avoid memory leaks
            if (wrapper == null || wrapper.IsTransient)
            {
                handle.Free();
            }

            return wrapper;
        }


        protected IntPtr ToIntPtr()
        {
            if (!handle.IsAllocated)
            {
                handle = GCHandle.Alloc(this, GCHandleType.Normal);
            }

            return GCHandle.ToIntPtr(handle);
        }


        public virtual void Dispose()
        {
            if (handle.IsAllocated)
            {
                handle.Free();
            }
        }
    }
}
