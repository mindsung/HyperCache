using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection;
using System.Collections.Concurrent;

namespace MindSung.HyperState
{
    public class ObjectProxy<TObject, TSerialized> : IObjectProxy<TObject, TSerialized> where TObject : class where TSerialized : class
    {
        internal ObjectProxy(ISerializationProvider<TSerialized> serializer)
        {
            this.serializer = serializer;
        }

        ISerializationProvider<TSerialized> serializer;
        TSerialized serialized;
        TObject obj;

        public TSerialized GetSerialized()
        {
            if (serialized == null && obj != null)
            {
                serialized = serializer.Serialize(obj);
            }
            return serialized;
        }

        public void SetSerialized(TSerialized serialized)
        {
            this.serialized = serialized;
            obj = null;
        }

        public TObject GetObject()
        {
            if (obj == null && serialized != null)
            {
                obj = serializer.Deserialize<TObject>(serialized);
            }
            return obj;
        }

        public void SetObject(TObject value)
        {
            this.obj = value;
            serialized = null;
        }
    }
}
