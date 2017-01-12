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
        IReadOnlyList<IObjectProxy<TObject, TSerialized>> FromSerializedCollection<TObject>(TSerialized serializedCollection);
        TSerialized ToSerializedCollection<TObject>(IEnumerable<IObjectProxy<TObject, TSerialized>> collection);
    }

    public interface IObjectProxyFactory : IObjectProxyFactory<string>
    {
        new IObjectProxy<TObject> FromObject<TObject>(TObject obj);
        new IObjectProxy<TObject> FromSerialized<TObject>(string serialized);
        new IReadOnlyList<IObjectProxy<TObject>> FromSerializedCollection<TObject>(string serializedCollection);
        string ToSerializedCollection<TObject>(IEnumerable<IObjectProxy<TObject>> collection);
    }
}
