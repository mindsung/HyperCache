using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindSung.HyperState
{
    public interface IObjectProxyFactory<TSerialized>
    {
        ISerializationProvider<TSerialized> Serializer { get; }
        IObjectProxy<TObject, TSerialized> FromObject<TObject>(TObject obj);
        IObjectProxy<TObject, TSerialized> FromSerialized<TObject>(TSerialized serialized);
        Type GetObjectProxyType<TObject>();
    }
}
