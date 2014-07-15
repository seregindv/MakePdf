using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MakePdf.Serialization
{
    interface ISerializationControl
    {
        void Serializing();
        void Serialized();
        void Deserialized();
    }
}
