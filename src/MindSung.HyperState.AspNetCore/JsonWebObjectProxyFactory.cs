using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Text;

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

        public IEnumerable<string> AcceptContentTypes { get { return accept; } }

        public Type GetObjectProxyType<TObject>()
        {
            return typeof(JsonObjectProxy<TObject>);
        }

        public async Task<string> ReadSerialized(HttpRequest request)
        {
            using (var reader = new StreamReader(request.Body, request.GetTypedHeaders().ContentType.Encoding))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async Task WriteSerialized(HttpResponse response, string serialized)
        {
            response.ContentType = "application/json";
            using (var writer = new StreamWriter(response.Body, Encoding.UTF8))
            {
                await writer.WriteAsync(serialized);
                await writer.FlushAsync();
            }
        }
    }
}
