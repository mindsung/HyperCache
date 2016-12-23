using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindSung.HyperState
{
    public class ObjectProxyFactory<TSerialized> where TSerialized : class
    {
        public ObjectProxyFactory(ISerializationProvider<TSerialized> serializer)
        {
            Serializer = serializer;
        }

        public ISerializationProvider<TSerialized> Serializer { get; private set; }

        public ObjectProxy<TObject, TSerialized> FromObject<TObject>(TObject obj) where TObject : class
        {
            var proxy = new ObjectProxy<TObject, TSerialized>(Serializer);
            proxy.SetObject(obj);
            return proxy;
        }

        public ObjectProxy<TObject, TSerialized> FromSerialized<TObject>(TSerialized serialized) where TObject : class
        {
            var proxy = new ObjectProxy<TObject, TSerialized>(Serializer);
            proxy.SetSerialized(serialized);
            return proxy;
        }
    }
}
