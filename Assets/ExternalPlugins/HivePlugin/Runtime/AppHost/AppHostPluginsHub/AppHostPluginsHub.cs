using System;
using System.Collections;
using System.Collections.Generic;


namespace Modules.Hive
{
    public partial class AppHostPluginsHub : IEnumerable<IAppHostPluginsHubChunk>
    {
        #region Fields
        
        private List<Chunk> chunks = new List<Chunk>();
        private Dictionary<Type, IAppHostPlugin> uniquePlugins = new Dictionary<Type, IAppHostPlugin>();
        private bool isSorted = true;
        
        #endregion
        
        

        #region Chunk tools

        private Chunk GetChunk(AppHostLayer layer, bool createIfNotExists)
        {
            Chunk chunk = chunks.Find(c => c.Layer == layer);
            if (createIfNotExists && chunk == null)
            {
                chunk = new Chunk(layer);
                chunks.Add(chunk);
                isSorted = false;
            }

            return chunk;
        }
        

        public IAppHostPluginsHubChunk GetChunk(AppHostLayer layer)
        {
            return GetChunk(layer, false);
        }
        

        public bool ContainsChunk(AppHostLayer layer)
        {
            return chunks.Find(chunk => chunk.Layer == layer) != null;
        }
        

        public void Clear()
        {
            uniquePlugins.Clear();
            chunks.Clear();
            isSorted = true;
        }
        

        public void Sort()
        {
            if (isSorted)
            {
                return;
            }

            // Insertion sort for partially sorted collection
            for (int i = 1; i < chunks.Count; i++)
            {
                Chunk chunk = chunks[i];
                int j = i - 1;
                while (j >= 0 && chunks[j].Layer < chunk.Layer)
                {
                    chunks[j + 1] = chunks[j];
                    j--;
                }

                j++;
                if (i != j)
                {
                    chunks[j] = chunk;
                }
            }

            isSorted = true;
        }

        
        public IEnumerator<IAppHostPluginsHubChunk> GetEnumerator()
        {
            Sort();
            return chunks.GetEnumerator();
        }
        

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        
        
        #region Plugin tools

        public void AddPlugin(IAppHostPlugin plugin)
        {
            if (plugin == null)
            {
                return;
            }

            Type pluginType = plugin.GetType();
            if (uniquePlugins.ContainsKey(pluginType))
            {
                throw new InvalidOperationException($"Failed to add plugin of type '{pluginType}' because it already exists.");
            }

            uniquePlugins.Add(pluginType, plugin);

            GetChunk(plugin.Layer, true)
                .Add(plugin);
        }
        

        public IAppHostPlugin GetPlugin(Type pluginType)
        {
            return uniquePlugins[pluginType];
        }
        

        public T GetPlugin<T>() where T : IAppHostPlugin
        {
            return (T)uniquePlugins[typeof(T)];
        }
        

        public bool ContainsPlugin(IAppHostPlugin plugin)
        {
            IAppHostPlugin existedPlugin = uniquePlugins[plugin.GetType()];
            return plugin == existedPlugin;
        }
        

        public bool ContainsPlugin(Type pluginType)
        {
            return uniquePlugins.ContainsKey(pluginType);
        }
        

        public bool ContainsPlugin<T>() where T : IAppHostPlugin
        {
            return uniquePlugins.ContainsKey(typeof(T));
        }
        

        public bool RemovePlugin(IAppHostPlugin plugin)
        {
            if (plugin == null)
            {
                return false;
            }

            Type pluginType = plugin.GetType();
            uniquePlugins.Remove(pluginType);

            // Remove the plugin from list (slow)
            Chunk chunk = GetChunk(plugin.Layer, false);
            if (chunk == null)
            {
                return false;
            }

            bool rs = chunk.Remove(plugin);

            if (chunk.Count == 0)
            {
                chunks.Remove(chunk);
                isSorted = false;
            }

            return rs;
        }
        

        public bool RemovePlugin(Type pluginType)
        {
            IAppHostPlugin plugin = GetPlugin(pluginType);
            if (plugin == null)
            {
                return false;
            }

            uniquePlugins.Remove(pluginType);

            // Remove the plugin from list (slow)
            Chunk chunk = GetChunk(plugin.Layer, false);
            if (chunk == null)
            {
                return false;
            }

            bool rs = chunk.Remove(pluginType);

            if (chunk.Count == 0)
            {
                chunks.Remove(chunk);
                isSorted = false;
            }

            return rs;
        }

        
        public bool RemovePlugin<T>() where T : IAppHostPlugin
        {
            return RemovePlugin(typeof(T));
        }

        #endregion
    }
}
