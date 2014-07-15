using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MakePdf.Pooling.Containers;

namespace MakePdf.Pooling.Storages
{
    public abstract class Storage<T> : IObjectStorage<T> where T : class
    {
        protected IObjectContainer<T> _container;
        protected Func<T> _createNew;

        protected Storage(IObjectContainer<T> container, Func<T> createNew)
        {
            _container = container;
            _createNew = createNew;
        }

        #region Implementation of IObjectStorage<T>

        public abstract T Get();

        public abstract void Return(T obj);

        #endregion
    }
}
