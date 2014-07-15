using System;
using MakePdf.Pooling.Containers;
using MakePdf.Pooling.Storages;
using System.Runtime.CompilerServices;

namespace MakePdf.Pooling.Pools
{
    public class ObjectPool<T> : IObjectPool<T>
    {
        readonly IObjectStorage<T> _storage;

        public ObjectPool(IObjectStorage<T> storage)
        {
            _storage = storage;
        }

        #region

        public DisposableObject<T> FetchDisposable()
        {
            return new DisposableObject<T>(this, Fetch());
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public T Fetch()
        {
            return _storage.Get();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Store(T obj)
        {
            _storage.Return(obj);
        }

        #endregion

        public static ObjectPool<T2> Get<T2>(Func<T2> createNew) where T2 : class
        {
            //return new ObjectPool<T2>(new UnlimitedStorage<T2>(new ListContainer<T2>(), createNew));
            return new ObjectPool<T2>(new SingleUseUnlimitedStorage<T2>(new SingleContainer<T2>(), createNew));
        }
    }
}
