using System.Collections.Generic;
using System.Threading;

namespace MakePdf.Pooling
{
    public class ThreadedPool<T>
    {
        readonly Dictionary<int, T> _instances = new Dictionary<int, T>();
        readonly object _syncRoot = new object();

        public T Get()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            T instance;
            if (!_instances.TryGetValue(threadId, out instance))
                lock (_syncRoot)
                    if (!_instances.TryGetValue(threadId, out instance))
                    {
                        instance = Create();
                        _instances.Add(threadId, instance);
                    }
            return instance;
        }

        protected virtual T Create()
        {
            return default(T);
        }
    }
}
