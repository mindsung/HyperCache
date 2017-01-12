using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindSung.HyperState
{
    public class JsonObjectProxy<T> : ObjectProxyBase<T>
    {
        internal JsonObjectProxy(ISerializationProvider<string> serializer)
            : base(serializer)
        {
        }
    }
}
