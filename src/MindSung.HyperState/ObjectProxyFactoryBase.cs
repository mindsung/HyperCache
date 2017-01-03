using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindSung.HyperState
{
    public abstract class ObjectProxyFactoryBase<TSerialized> : IObjectProxyFactory<TSerialized>
    {
        protected ObjectProxyFactoryBase(ISerializationProvider<TSerialized> serializer)
        {
            Serializer = serializer;
        }

        protected abstract IObjectProxy<TObject, TSerialized> CreateObjectProxy<TObject>(ISerializationProvider<TSerialized> serializer);

        public ISerializationProvider<TSerialized> Serializer { get; private set; }

        public virtual IObjectProxy<TObject, TSerialized> FromObject<TObject>(TObject obj)
        {
            var proxy = CreateObjectProxy<TObject>(Serializer);
            proxy.Object = obj;
            return proxy;
        }

        public virtual IObjectProxy<TObject, TSerialized> FromSerialized<TObject>(TSerialized serialized)
        {
            var proxy = CreateObjectProxy<TObject>(Serializer);
            proxy.Serialized = serialized;
            return proxy;
        }

        public abstract Type GetObjectProxyType<TObject>();

        public virtual IReadOnlyList<IObjectProxy<TObject, TSerialized>> FromSerializedCollection<TObject>(TSerialized serializedCollection)
        {
            throw new NotImplementedException();
        }

        public virtual TSerialized ToSerializedCollection<TObject>(IEnumerable<IObjectProxy<TObject, TSerialized>> collection)
        {
            throw new NotImplementedException();
        }
    }
}
