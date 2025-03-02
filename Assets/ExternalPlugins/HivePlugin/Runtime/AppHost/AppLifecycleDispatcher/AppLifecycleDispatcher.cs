using System;
using System.Collections.Generic;


namespace Modules.Hive
{
    internal class AppLifecycleDispatcher : IAppLifecycleDispatcher
    {
        #region Fields
        
        private List<IAppLifecycleHandlerDescriptor> descriptors = new List<IAppLifecycleHandlerDescriptor>();
        private List<IAppLifecycleHandlerDescriptor> descriptorsToAdd;
        private List<IAppLifecycleHandlerDescriptor> descriptorsToRemove;
        private bool isSorted = false;
        private bool isInProgress = false;
        
        #endregion
        


        #region IAppLifecycleDispatcher implementation

        public int Count => descriptors.Count;


        public void Add(IAppLifecycleHandlerDescriptor descriptor)
        {
            if (isInProgress)
            {
                if (descriptorsToAdd == null)
                {
                    descriptorsToAdd = new List<IAppLifecycleHandlerDescriptor>();
                }

                descriptorsToAdd.Add(descriptor);
                return;
            }

            AddDescriptor(descriptor);
        }


        public void Remove(IAppLifecycleHandlerDescriptor descriptor)
        {
            if (isInProgress)
            {
                if (descriptorsToRemove == null)
                {
                    descriptorsToRemove = new List<IAppLifecycleHandlerDescriptor>();
                }

                descriptorsToRemove.Add(descriptor);
                return;
            }

            RemoveDescriptor(descriptor);
        }


        public void Remove(IAppLifecycleHandler handler)
        {
            if (TryGetDescriptor(handler, out var descriptor))
            {
                Remove(descriptor);
            }
        }


        public bool Contains(IAppLifecycleHandlerDescriptor descriptor)
        {
            return descriptors.Contains(descriptor);
        }


        public bool Contains(IAppLifecycleHandler handler)
        {
            return TryGetDescriptor(handler, out var _);
        }


        public bool TryGetDescriptor(IAppLifecycleHandler handler, out IAppLifecycleHandlerDescriptor descriptor)
        {
            descriptor = null;
            int count = descriptors.Count;
            for (int i = 0; i < count; i++)
            {
                var item = descriptors[i];
                if (item.Handler == handler)
                {
                    descriptor = item;
                    return true;
                }
            }

            return false;
        }

        #endregion



        #region Descriptor management

        private void AddDescriptor(IAppLifecycleHandlerDescriptor descriptor)
        {
            if (Contains(descriptor.Handler))
            {
                return;
            }

            descriptors.Add(descriptor);
            isSorted = false;
        }


        private void RemoveDescriptor(IAppLifecycleHandlerDescriptor descriptor)
        {
            if (descriptors.Remove(descriptor))
            {
                isSorted = false;
            }
        }


        private void ApplyDefferedChanges()
        {
            if (descriptorsToRemove != null)
            {
                int count = descriptorsToRemove.Count;
                for (int i = 0; i < count; i++)
                {
                    RemoveDescriptor(descriptorsToRemove[i]);
                }

                descriptorsToRemove = null;
            }

            if (descriptorsToAdd != null)
            {
                int count = descriptorsToAdd.Count;
                for (int i = 0; i < count; i++)
                {
                    AddDescriptor(descriptorsToAdd[i]);
                }

                descriptorsToAdd = null;
            }
        }


        private void SortIfRequired(bool forced = false)
        {
            if (isSorted && !forced)
            {
                return;
            }

            descriptors.Sort(new AppLifecycleHandlerDescriptorComparer());

            isSorted = true;
        }

        #endregion



        #region Enumeration

        private void BeginEnumerate()
        {
            isInProgress = true;
            SortIfRequired();
        }


        private void EndEnumerate()
        {
            ApplyDefferedChanges();
            isInProgress = false;
        }


        internal void EnumerateForward(Action<IAppLifecycleHandler> action)
        {
            if (action == null)
            {
                return;
            }

            BeginEnumerate();
            for (int i = 0; i < Count; i++)
            {
                action(descriptors[i].Handler);
            }

            EndEnumerate();
        }


        internal void EnumerateBackward(Action<IAppLifecycleHandler> action)
        {
            if (action == null)
            {
                return;
            }

            BeginEnumerate();
            for (int i = Count - 1; i >= 0; i--)
            {
                action(descriptors[i].Handler);
            }

            EndEnumerate();
        }


        internal void EnumerateBackwardSafely(Action<IAppLifecycleHandler> action, List<Exception> exceptions)
        {
            if (action == null)
            {
                return;
            }

            BeginEnumerate();
            for (int i = Count - 1; i >= 0; i--)
            {
                try
                {
                    action(descriptors[i].Handler);
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }
            EndEnumerate();
        }

        #endregion
    }
}
