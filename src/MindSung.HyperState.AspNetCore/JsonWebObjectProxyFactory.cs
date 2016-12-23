using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace MindSung.HyperState.AspNetCore
{
    public class JsonWebObjectProxyFactory : JsonObjectProxyFactory, IWebObjectProxyFactory<string>
    {
        public JsonWebObjectProxyFactory(ISerializationProvider<string> jsonSerializer)
            : base(jsonSerializer)
        {
        }

        public JsonWebObjectProxyFactory(JsonSerializerSettings settings = null)
            : base(settings)
        {
        }

        public JsonWebObjectProxyFactory(Action<JsonSerializerSettings> setupAction)
            : base(setupAction)
        {
        }

        private readonly string[] accept = { "application/json" };

        public IEnumerable<string> AcceptContectTypes { get { return accept; } }

        private readonly string contentType = "application/json";

        public string OutputContentType { get { return contentType; } }

        public Type GetObjectProxyType<TObject>()
        {
            return typeof(JsonObjectProxy<TObject>);
        }

        public async Task<string> ReadAsSerialized(Stream stream)
        {
            using (var textReader = new StreamReader(stream))
            {
                return await textReader.ReadToEndAsync();
            }
        }
    }
}
