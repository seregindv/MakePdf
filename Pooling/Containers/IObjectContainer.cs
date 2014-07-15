using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MakePdf.Pooling.Containers
{
    public interface IObjectContainer<T> : ICollection<T>
    {
        T Get();
    }
}
