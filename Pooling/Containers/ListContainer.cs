using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace MakePdf.Pooling.Containers
{
    public class ListContainer<T> : List<T>, IObjectContainer<T> where T : class
    {
        #region IObjectContainer

        public T Get()
        {
            if (Count == 0)
                return null;
            var result = this[Count - 1];
            Remove(result);
            return result;
        }

        #endregion
    }
}
