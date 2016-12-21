using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection;
using System.Collections.Concurrent;

namespace MindSung.HyperState
{
    public class ObjectProxy<T> where T : class
    {
        string serialized;
        T value;

        public string GetSerialized()
        {
            if (serialized == null && value != null)
            {
                serialized = JsonConvert.SerializeObject(value);
            }
            return serialized;
        }

        public void SetSerialized(string serialized)
        {
            this.serialized = serialized;
            value = null;
        }

        public T GetObject()
        {
            if (value == null && serialized != null)
            {
                value = JsonConvert.DeserializeObject<T>(serialized);
            }
            return value;
        }

        public void SetObject(T value)
        {
            this.value = value;
            serialized = null;
        }
    }
}
