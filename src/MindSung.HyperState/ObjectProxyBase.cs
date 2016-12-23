using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection;
using System.Collections.Concurrent;

namespace MindSung.HyperState
{
    public abstract class ObjectProxyBase<TObject, TSerialized> : IObjectProxy<TObject, TSerialized>
    {
        protected ObjectProxyBase(ISerializationProvider<TSerialized> serializer)
        {
            this.serializer = serializer;
        }

        private ISerializationProvider<TSerialized> serializer;
        private TSerialized serialized;
        private bool hasSerialized;
        private TObject obj;
        private bool hasObj;
        private object sync = new object();

        public virtual TSerialized GetSerialized()
        {
            if (!hasObj && !hasSerialized)
            {
                throw new InvalidOperationException("The object proxy has not been initialized.");
            }
            if (!hasSerialized)
            {
                lock (sync)
                {
                    if (!hasSerialized)
                    {
                        serialized = serializer.Serialize(obj);
                        hasSerialized = true;
                    }
                }
            }
            return serialized;
        }

        public virtual void SetSerialized(TSerialized serialized)
        {
            lock (sync)
            {
                this.serialized = serialized;
                obj = default(TObject);
                hasObj = false;
                hasSerialized = true;
            }
        }

        public virtual TObject GetObject()
        {
            if (!hasObj && !hasSerialized)
            {
                throw new InvalidOperationException("The object proxy has not been initialized.");
            }
            if (!hasObj)
            {
                lock (sync)
                {
                    if (!hasObj)
                    {
                        obj = serializer.Deserialize<TObject>(serialized);
                        hasObj = true;
                    }
                }
            }
            return obj;
        }

        public virtual void SetObject(TObject obj)
        {
            lock (sync)
            {
                this.obj = obj;
                serialized = default(TSerialized);
                hasSerialized = false;
                hasObj = true;
            }
        }
    }
}
