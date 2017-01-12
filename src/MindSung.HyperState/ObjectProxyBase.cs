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
        private object sync = new object();

        public virtual TSerialized Serialized
        {
            get
            {
                if (!hasObject && !hasSerialized)
                {
                    throw new InvalidOperationException("The object proxy has not been initialized.");
                }
                if (!hasSerialized)
                {
                    lock (sync)
                    {
                        if (!hasSerialized)
                        {
                            _Serialized = serializer.Serialize(_Object);
                            hasSerialized = true;
                        }
                    }
                }
                return _Serialized;
            }
            set
            {
                lock (sync)
                {
                    this._Serialized = value;
                    _Object = default(TObject);
                    hasObject = false;
                    hasSerialized = true;
                }
            }
        }
        private TSerialized _Serialized;
        private bool hasSerialized;

        public virtual TObject Object
        {
            get
            {
                if (!hasObject && !hasSerialized)
                {
                    throw new InvalidOperationException("The object proxy has not been initialized.");
                }
                if (!hasObject)
                {
                    lock (sync)
                    {
                        if (!hasObject)
                        {
                            _Object = serializer.Deserialize<TObject>(_Serialized);
                            hasObject = true;
                        }
                    }
                }
                return _Object;
            }
            set
            {
                lock (sync)
                {
                    this._Object = value;
                    hasObject = true;
                    ObjectChanged();
                }
            }
        }
        private TObject _Object;
        private bool hasObject;

        public void ObjectChanged()
        {
            _Serialized = default(TSerialized);
            hasSerialized = false;
        }
    }

    public abstract class ObjectProxyBase<TObject> : ObjectProxyBase<TObject, string>, IObjectProxy<TObject>
    {
        protected ObjectProxyBase(ISerializationProvider<string> serializer)
            : base(serializer)
        {
        }
    }
}
