using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindSung.HyperState
{
    public interface ISerializationProvider<TSerialized>
    {
        TSerialized Serialize<T>(T obj);
        TSerialized Serialize(object obj, Type type);
        T Deserialize<T>(TSerialized serialized);
        object Deserialize(TSerialized serialized, Type type);
    }
}
