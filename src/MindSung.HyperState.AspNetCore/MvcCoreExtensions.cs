using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace MindSung.HyperState.AspNetCore
{
    public static class MvcCoreExtensions
    {
        private static ConcurrentDictionary<string, object> defaultFactories = new ConcurrentDictionary<string, object>();

        public static IMvcCoreBuilder AddWebObjectProxy<TSerialized>(this IMvcCoreBuilder builder, IWebObjectProxyFactory<TSerialized> factory)
        {
            // Always add the factory for the default IWebObjectProxyFactory interface.
            if (defaultFactories.TryAdd("stringfactory", factory))
            {
                builder.Services.AddSingleton(typeof(IWebObjectProxyFactory), factory);
            }
            // If this is the first factory added for the TSerialized type, set it as the default factory
            // for dependency injection of the generic type IWebObjectProxyFactory<TSerialized>
            if (defaultFactories.TryAdd(typeof(TSerialized).FullName, factory))
            {
                builder.Services.AddSingleton(typeof(IWebObjectProxyFactory<TSerialized>), factory);
            }
            // Add the factory for dependency injection of the specific type.
            builder.Services.AddSingleton(factory.GetType(), factory);
            // Add formatters.
            return builder.AddMvcOptions(options =>
            {
                var formatter = new ObjectProxyFormatter<TSerialized>(factory, options);
                options.OutputFormatters.Insert(0, formatter);
                options.InputFormatters.Insert(0, formatter);
            });
        }

        public static IMvcCoreBuilder AddWebObjectProxy(this IMvcCoreBuilder builder, IWebObjectProxyFactory factory)
        {
            return AddWebObjectProxy<string>(builder, factory);
        }

        public static IMvcCoreBuilder AddJsonWebObjectProxy(this IMvcCoreBuilder builder, ISerializationProvider<string> jsonSerializer)
        {
            return AddWebObjectProxy(builder, new JsonWebObjectProxyFactory(jsonSerializer));
        }

        public static IMvcCoreBuilder AddJsonWebObjectProxy(this IMvcCoreBuilder builder, JsonSerializerSettings settings = null)
        {
            return AddWebObjectProxy(builder, new JsonWebObjectProxyFactory(settings));
        }

        public static IMvcCoreBuilder AddJsonWebObjectProxy(this IMvcCoreBuilder builder, Action<JsonSerializerSettings> setupAction)
        {
            return AddWebObjectProxy(builder, new JsonWebObjectProxyFactory(setupAction));
        }
    }
}
