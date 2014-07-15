using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MakePdf.Pooling.Containers;

namespace MakePdf.Pooling.Storages
{
    public class UnlimitedStorage<T> : Storage<T> where T : class
    {
        public UnlimitedStorage(IObjectContainer<T> container, Func<T> createNew)
            : base(container, createNew)
        {
        }

        #region Overrides of Storage<T>

        public override T Get()
        {
            var result = _container.Get();
            if (result == null)
            {
                result = _createNew();
                _container.Add(result);
                result = _container.Get();
                System.Diagnostics.Debug.WriteLine("Added");
            }
            return result;
        }

        public override void Return(T obj)
        {
            _container.Add(obj);
            System.Diagnostics.Debug.WriteLine("Returned");
        }

        #endregion
    }
}
