using System;
using System.Collections;
using System.Collections.Generic;


namespace Modules.Hive
{
    partial class AppHostPluginsHub
    {
        private class Chunk : IAppHostPluginsHubChunk
        {
            #region Fields
            
            private List<IAppHostPlugin> plugins = new List<IAppHostPlugin>();
            
            #endregion
            
            
            
            #region Properties

            public AppHostLayer Layer { get; }

            public int Count => plugins.Count;
            
            #endregion
            
            
            
            #region Class lifecycle
            
            public Chunk(AppHostLayer layer)
            {
                Layer = layer;
            }
            
            #endregion
            
            
            
            #region Methods

            public void Add(IAppHostPlugin plugin)
            {
                plugins.Add(plugin);
            }
            

            public IAppHostPlugin Get(Type pluginType)
            {
                if (pluginType != null)
                {
                    for (int i = 0; i < plugins.Count; i++)
                    {
                        IAppHostPlugin plugin = plugins[i];
                        if (plugin.GetType() == pluginType)
                        {
                            return plugin;
                        }
                    }
                }

                return null;
            }

            
            public T Get<T>() where T : class, IAppHostPlugin
            {
                return Get(typeof(T)) as T;
            }
            

            public bool Remove(IAppHostPlugin plugin) => plugins.Remove(plugin);
            

            public bool Remove(Type pluginType)
            {
                for (int i = 0; i < plugins.Count; i++)
                {
                    if (plugins[i].GetType() == pluginType)
                    {
                        plugins.RemoveAt(i);
                        return true;
                    }
                }

                return false;
            }
            

            public bool Remove<T>() where T : class, IAppHostPlugin => Remove(typeof(T));
            

            public void Clear()
            {
                plugins.Clear();
            }
            

            public IEnumerator<IAppHostPlugin> GetEnumerator() => plugins.GetEnumerator();
            
            
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            
            #endregion

        }
    }
}
