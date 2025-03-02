using System;
using System.Collections.Generic;


namespace Modules.Hive
{
    public interface IAppHostPluginsHubChunk : IEnumerable<IAppHostPlugin>
    {
        AppHostLayer Layer { get; }

        int Count { get; }
        IAppHostPlugin Get(Type pluginType);
        T Get<T>() where T : class, IAppHostPlugin;
    }
}
