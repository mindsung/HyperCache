using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace MindSung.HyperState
{
    public class JsonObjectProxyFactory : ObjectProxyFactoryBase<string>
    {
        public JsonObjectProxyFactory(ISerializationProvider<string> jsonSerializer)
            : base(jsonSerializer)
        {
        }

        public JsonObjectProxyFactory(JsonSerializerSettings settings = null)
            : this(new JsonSerializationProvider(settings))
        {
        }

        public JsonObjectProxyFactory(Action<JsonSerializerSettings> setupAction)
            : this(new JsonSerializationProvider(setupAction))
        {
        }

        protected override IObjectProxy<TObject, string> CreateObjectProxy<TObject>(ISerializationProvider<string> serializer)
        {
            return new JsonObjectProxy<TObject>(Serializer);
        }
    }
}
