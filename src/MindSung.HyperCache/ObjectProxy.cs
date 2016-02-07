using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindSung.HyperCache
{
    public class ObjectProxy<T>
    {
        public static ObjectProxy<T> FromValue(string key, T value)
        {
            if (string.IsNullOrWhiteSpace(key)) { throw new ArgumentException("Invalid object key", "key"); }
            return new ObjectProxy<T>() { Key = key, _Value = value };
        }

        public static ObjectProxy<T> FromSerialized(string key, string serialized)
        {
            if (string.IsNullOrWhiteSpace(key)) { throw new ArgumentException("Invalid object key", "key"); }
            return new ObjectProxy<T>() { Key = key, _Serialized = serialized };
        }

        private ObjectProxy()
        {
        }

        object sync = new object();

        public string Key { get; private set; }

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
        string _Serialized;
        bool hasSerialized;
    }
}
