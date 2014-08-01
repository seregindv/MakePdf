using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MakePdf.Pooling.Containers;
using System.Threading;

namespace MakePdf.Pooling.Storages
{
    public class LimitedStorage<T> : Storage<T> where T : class
    {
        readonly AutoResetEvent _signal = new AutoResetEvent(true);
        readonly int _maxCount;
        int _count;

        public LimitedStorage(IObjectContainer<T> container, Func<T> createNew, int maxCount)
            : base(container, createNew)
        {
            _maxCount = maxCount;
        }

        #region Overrides of Storage<T>

        public override T Get()
        {
            if (_container.Count > 0)
                return _container.Get();
            if (_count >= _maxCount)
            {
                _signal.Reset();
                _signal.WaitOne();
                if (_container.Count > 0)
                    return _container.Get();
            }
            var result = _createNew();
            _container.Add(result);
            _count++;
            return _container.Get();
        }

        public override void Return(T obj)
        {
            _container.Add(obj);
            _signal.Set();
        }

        #endregion
    }
}
