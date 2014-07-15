using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MakePdf.Pooling.Containers;

namespace MakePdf.Pooling.Storages
{
    public class SingleUseUnlimitedStorage<T> : UnlimitedStorage<T> where T : class
    {
        public SingleUseUnlimitedStorage(IObjectContainer<T> container, Func<T> createNew)
            : base(container, createNew)
        {
        }

        public override void Return(T obj)
        {
        }
    }
}
