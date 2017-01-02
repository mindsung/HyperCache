using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System.Reflection;

namespace MindSung.HyperState
{
    class JsonSerializationProvider : ISerializationProvider<string>
    {
        public JsonSerializationProvider(JsonSerializerSettings settings = null)
        {
            Settings = JsonConvert.DefaultSettings?.Invoke() ?? new JsonSerializerSettings();
        }

        public JsonSerializationProvider(Action<JsonSerializerSettings> setupAction)
            : this()
        {
            setupAction?.Invoke(Settings);
        }

        public JsonSerializerSettings Settings { get; private set; }

        public string Serialize<T>(T obj)
        {
            if (obj == null)
            {
                return "";
            }
            return JsonConvert.SerializeObject(obj, Settings);
        }

        public string Serialize(object obj, Type type)
        {
            if (obj == null)
            {
                return "";
            }
            return JsonConvert.SerializeObject(obj, type, Settings);
        }

        public T Deserialize<T>(string serialized)
        {
            if (string.IsNullOrWhiteSpace(serialized))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(serialized, Settings);
        }

        public object Deserialize(string serialized, Type type)
        {
            if (string.IsNullOrWhiteSpace(serialized))
            {
                if (type.GetTypeInfo().IsValueType)
                {
                    return Activator.CreateInstance(type);
                }
                return null;
            }
            return JsonConvert.DeserializeObject(serialized, type, Settings);
        }
    }
}
