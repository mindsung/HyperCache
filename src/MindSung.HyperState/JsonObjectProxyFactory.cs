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

        public new JsonObjectProxy<TObject> FromObject<TObject>(TObject obj)
        {
            return (JsonObjectProxy<TObject>)base.FromObject<TObject>(obj);
        }

        public new JsonObjectProxy<TObject> FromSerialized<TObject>(string serialized)
        {
            return (JsonObjectProxy<TObject>)base.FromSerialized<TObject>(serialized);
        }
    }
}
