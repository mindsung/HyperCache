using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindSung.HyperState
{
    public interface IDualStateFactory<TSerialized>
    {
        ISerializationProvider<TSerialized> Serializer { get; }
        IDualState<TObject, TSerialized> FromObject<TObject>(TObject obj);
        IDualState<TObject, TSerialized> FromSerialized<TObject>(TSerialized serialized);
        Type GetDualStateObjectType<TObject>();
        IReadOnlyList<IDualState<TObject, TSerialized>> FromSerializedCollection<TObject>(TSerialized serializedCollection);
        TSerialized ToSerializedCollection<TObject>(IEnumerable<IDualState<TObject, TSerialized>> collection);
    }

    public interface IDualStateFactory : IDualStateFactory<string>
    {
        new IDualState<TObject> FromObject<TObject>(TObject obj);
        new IDualState<TObject> FromSerialized<TObject>(string serialized);
        new IReadOnlyList<IDualState<TObject>> FromSerializedCollection<TObject>(string serializedCollection);
        string ToSerializedCollection<TObject>(IEnumerable<IDualState<TObject>> collection);
    }
}
