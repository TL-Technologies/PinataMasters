using System;
using System.Collections.Generic;
using System.Linq;


namespace Modules.General
{
    public class DataState
    {
        #region Nested types
        
        private class StateLeaf
        {
            #region Fields
        
            private Dictionary<string, object> values = new Dictionary<string, object>();
            private Dictionary<string, StateLeaf> leaves = new Dictionary<string, StateLeaf>();
            private StateLeaf parent = null;

            #endregion
        


            #region Methods
        
            public void SetParent(StateLeaf parent)
            {
                this.parent = parent;
            }


            public bool TryGet<T>(string valueName, out T value)
            {
                var parts = valueName.Split('.');
                var current = this;
                value = default;

                for (int i = 0; i < parts.Length; ++i)
                {
                    if (i == parts.Length - 1)
                    {
                        if (current == null)
                        {
                            return false;
                        }

                        if (!current.values.ContainsKey(parts[i]))
                        {
                            return false;
                        }

                        value = (T)current.values[parts[i]];
                        return true;
                    }

                    if (current == null) return false;
                    current = current.GetLeaf(parts[i]);
                }

                return false;
            }


            public T Get<T>(string valueName)
            {
                return TryGet(valueName, out T value) ? value : default;
            }


            public void Set<T>(string valueName, T value)
            {
                var current = this;
                var parts = valueName.Split('.');
                for (int i = 0; i < parts.Length; ++i)
                {
                    if (i == parts.Length - 1)
                    {
                        current.values[parts[i]] = value;  // TODO: Resolve possible boxing?
                    }
                    else
                    {
                        StateLeaf last = current;
                        current = current.GetLeaf(parts[i]);
                        if (current == null)
                        {
                            current = new StateLeaf();
                            last.AddLeaf(parts[i], current);
                        }
                    }
                }
            }

            
            public void Remove(string valueName)
            {
                var parts = valueName.Split('.');
                var current = this;

                for (int i = 0; i < parts.Length; ++i)
                {
                    if (i == parts.Length - 1)
                    {
                        if (current == null)
                        {
                            return;
                        }

                        if (!current.values.ContainsKey(parts[i]))
                        {
                            return;
                        }

                        current.values.Remove(parts[i]);
                        current.leaves.Remove(parts[i]);
                    }
                    else
                    {
                        if (current == null) return;
                        current = current.GetLeaf(parts[i]);
                    }
                }
            }


            private StateLeaf GetLeaf(string leafName)
            {
                return leaves.ContainsKey(leafName) ? leaves[leafName] : null;
            }


            private void AddLeaf(string leafName, StateLeaf leaf)
            {
                if (leaves.ContainsKey(leafName)) return;
                leaf.parent = this;
                leaves[leafName] = leaf;
            }

            #endregion
        }

        
        private class StateListener
        {
            #region Properties
            
            public Action<object, object> UpdateCallback { get; }
        
            
            public string GroupName { get; }
            
            
            public string ListenValue { get; }
        
            #endregion



            #region Class lifecycle
        
            public StateListener(string groupName, string valueName, Action<object, object> callback)
            {
                GroupName = groupName;
                ListenValue = valueName;
                UpdateCallback = callback;
            }
        
            #endregion
        }

        #endregion
        
        
        
        #region Fields

        private List<StateListener> listeners = new List<StateListener>();
        private StateLeaf leaf = new StateLeaf();
        private string currentGroup = "default";

        #endregion



        #region Methods
        
        public void Set<T>(string valueName, T value)
        {
            var oldValue = leaf.Get<T>(valueName);
            leaf.Set(valueName, value);

            var watchers = GetListeners(valueName);
            foreach (var watcher in watchers)
            {
                watcher.UpdateCallback(oldValue, value);
            }
        }
        
        
        public bool TryGet<T>(string valueName, out T value)
        {
            return leaf.TryGet(valueName, out value);
        }

        
        public T Get<T>(string valueName)
        {
            return leaf.Get<T>(valueName);
        }
        
        
        public void Remove(string valueName)
        {
            leaf.Remove(valueName);
        }
        
        
        public void AddListener(string valueName, Action<object, object> callback)
        {
            var listener = new StateListener(currentGroup, valueName, callback);
            listeners.Add(listener);
        }


        public void RemoveListener(string valueName)
        {
            listeners.Remove(listeners.Find(listener => listener.ListenValue == valueName));
        }

        
        private StateListener[] GetListeners(string valueName)
        {
            return listeners.Where(listener => listener.ListenValue == valueName).ToArray();
        }


        public void Listen(string groupName)
        {
            currentGroup = groupName;
        }

        
        public void Stop(string groupName)
        {
            listeners.RemoveAll((x) => x.GroupName == groupName);
        }

        #endregion
    }
}
