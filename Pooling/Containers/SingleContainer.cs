using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MakePdf.Pooling.Containers
{
    public class SingleContainer<T> : IObjectContainer<T> where T : class
    {
        T _object;

        public T Get()
        {
            var obj = _object;
            _object = null;
            return obj;
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            _object = item;
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public int Count { get; private set; }
        public bool IsReadOnly { get; private set; }
    }
}
