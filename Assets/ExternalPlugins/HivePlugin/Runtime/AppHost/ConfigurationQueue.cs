using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Modules.Hive
{
    public class ConfigurationQueue<T>
    {
        private Queue<Func<T, Task>> queue = new Queue<Func<T, Task>>();

        
        public void Add(Func<T, Task> action)
        {
            queue.Enqueue(action);
        }
        

        public void Add(Action<T> action)
        {
            queue.Enqueue(config =>
            {
                action(config);
                return Task.CompletedTask;
            });
        }
        

        public async Task Process(T config)
        {
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                await c(config);
            }
        }
    }
}
