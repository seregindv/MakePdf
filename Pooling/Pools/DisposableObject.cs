using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MakePdf.Pooling.Pools
{
    public class DisposableObject<T> : IDisposable
    {
        readonly ObjectPool<T> _pool;

        public DisposableObject(ObjectPool<T> pool, T theObject)
        {
            _pool = pool;
            Object = theObject;
        }

        public T Object { get; private set; }

        public void Dispose()
        {
            _pool.Store(Object);
        }
    }
}
