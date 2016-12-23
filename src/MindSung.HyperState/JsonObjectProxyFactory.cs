using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace MindSung.HyperState
{
    public class JsonObjectProxyFactory : ObjectProxyFactory<string>
    {
        public JsonObjectProxyFactory(Action<JsonSerializerSettings> setupAction = null)
            : base(new JsonSerializationProvider(setupAction))
        {
        }
    }
}
