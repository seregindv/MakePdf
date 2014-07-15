using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MakePdf.Pooling
{
    public class DelegateThreadedPool<T> : ThreadedPool<T>
    {
        Func<T> _createFunc { set; get; }

        public DelegateThreadedPool(Func<T> createFunc)
        {
            _createFunc = createFunc;
        }

        protected override T Create()
        {
            return _createFunc == null
                ? base.Create()
                : _createFunc();
        }
    }
}
