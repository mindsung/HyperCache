using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindSung.HyperCache
{
    public class ObjectProxy<T>
    {
        public ObjectProxy(T value)
        {
            if (value == null) { throw new ArgumentException("ObjectProxy value cannot be null", "value"); }
        }

        public ObjectProxy(string serialized)
        {
            if (serialized == null) { throw new ArgumentException("ObjectProxy serialized value cannot be null", "serialized"); }
        }

        object sync = new object();

        public T Value
        {
            get
            {
                if (!hasValue)
                {
                    lock (sync)
                    {
                        if (!hasValue)
                        {
                            // TODO: Deserialize
                            hasValue = true;
                        }
                    }
                }
                return _Value;
            }
        }
        T _Value;
        bool hasValue;

        public string Serialized
        {
            get
            {
                if (!hasSerialized)
                {
                    lock (sync)
                    {
                        if (!hasSerialized)
                        {
                            // TODO: Serialize
                            hasSerialized = true;
                        }
                    }
                }
                return _Serialized;
            }
        }
        string _Serialized = null;
        bool hasSerialized;
    }
}
