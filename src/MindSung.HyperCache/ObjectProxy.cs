using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindSung.HyperCache
{
    public class ObjectProxy<T> where T : class, IEnumerable<KeyValuePair<string, object>>
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
                if (_Value == null)
                {
                    lock (sync)
                    {
                        if (_Value == null)
                        {
                            // TODO: Deserialize
                        }
                    }
                }
                return _Value;
            }
        }
        T _Value = null;

        public string Serialized
        {
            get
            {
                if (_Serialized == null)
                {
                    lock (sync)
                    {
                        if (_Serialized == null)
                        {
                            // TODO: Serialize
                        }
                    }
                }
                return _Serialized;
            }
        }
        string _Serialized = null;
    }
}
