using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

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
            return JsonConvert.SerializeObject(obj, Settings);
        }

        public string Serialize(object obj, Type type)
        {
            return JsonConvert.SerializeObject(obj, type, Settings);
        }

        public T Deserialize<T>(string serialized)
        {
            return JsonConvert.DeserializeObject<T>(serialized, Settings);
        }

        public object Deserialize(string serialized, Type type)
        {
            return JsonConvert.DeserializeObject(serialized, type, Settings);
        }
    }
}
