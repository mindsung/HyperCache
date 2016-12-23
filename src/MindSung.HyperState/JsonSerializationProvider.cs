using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace MindSung.HyperState
{
    class JsonSerializationProvider : ISerializationProvider<string>
    {
        public JsonSerializationProvider(Action<JsonSerializerSettings> setupAction = null)
        {
            Settings = JsonConvert.DefaultSettings?.Invoke() ?? new JsonSerializerSettings();
            setupAction?.Invoke(Settings);
        }

        public JsonSerializerSettings Settings { get; private set; }

        public T Deserialize<T>(string serialized)
        {
            return JsonConvert.DeserializeObject<T>(serialized, Settings);
        }

        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, Settings);
        }
    }
}
