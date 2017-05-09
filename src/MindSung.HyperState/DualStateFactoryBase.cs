using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindSung.HyperState
{
    public abstract class DualStateFactoryBase<TSerialized> : IDualStateFactory<TSerialized>
    {
        protected DualStateFactoryBase(ISerializationProvider<TSerialized> serializer)
        {
            Serializer = serializer;
        }

        protected abstract IDualState<TObject, TSerialized> CreateDualState<TObject>(ISerializationProvider<TSerialized> serializer);

        public ISerializationProvider<TSerialized> Serializer { get; private set; }

        public virtual IDualState<TObject, TSerialized> FromObject<TObject>(TObject obj)
        {
            var proxy = CreateDualState<TObject>(Serializer);
            proxy.Object = obj;
            return proxy;
        }

        public virtual IDualState<TObject, TSerialized> FromSerialized<TObject>(TSerialized serialized)
        {
            var proxy = CreateDualState<TObject>(Serializer);
            proxy.Serialized = serialized;
            return proxy;
        }

        public abstract Type GetDualStateObjectType<TObject>();

        public virtual IReadOnlyList<IDualState<TObject, TSerialized>> FromSerializedCollection<TObject>(TSerialized serializedCollection)
        {
            throw new NotImplementedException();
        }

        public virtual TSerialized ToSerializedCollection<TObject>(IEnumerable<IDualState<TObject, TSerialized>> collection)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class DualStateFactoryBase : DualStateFactoryBase<string>, IDualStateFactory
    {
        protected DualStateFactoryBase(ISerializationProvider<string> serializer)
            : base(serializer)
        {
        }

        IDualState<TObject> IDualStateFactory.FromObject<TObject>(TObject obj)
        {
            return (IDualState<TObject>)FromObject(obj);
        }

        IDualState<TObject> IDualStateFactory.FromSerialized<TObject>(string serialized)
        {
            return (IDualState<TObject>)FromSerialized<TObject>(serialized);
        }

        IReadOnlyList<IDualState<TObject>> IDualStateFactory.FromSerializedCollection<TObject>(string serializedCollection)
        {
            return (IReadOnlyList<IDualState<TObject>>)FromSerializedCollection<TObject>(serializedCollection);
        }

        string IDualStateFactory.ToSerializedCollection<TObject>(IEnumerable<IDualState<TObject>> collection)
        {
            return ToSerializedCollection(collection);
        }
    }
}
