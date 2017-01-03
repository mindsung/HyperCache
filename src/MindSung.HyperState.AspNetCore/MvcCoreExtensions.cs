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
        private static ConcurrentDictionary<Type, object> defaultFactories = new ConcurrentDictionary<Type, object>();

        public static IMvcCoreBuilder AddWebObjectProxy<TSerialized, TFactory>(this IMvcCoreBuilder builder, TFactory factory)
            where TFactory : IWebObjectProxyFactory<TSerialized>
        {
            // If this is the first factory added for the TSerialized type, set it as the default factory
            // for dependency injection of the generic type IWebObjectProxyFactory<TSerialized>
            if (defaultFactories.TryAdd(typeof(TSerialized), factory))
            {
                builder.Services.AddSingleton(typeof(IWebObjectProxyFactory<TSerialized>), _ => defaultFactories[typeof(TSerialized)]);
            }
            // Add the factory for dependency injection of the specific type.
            builder.Services.AddSingleton(factory.GetType(), factory);
            // Add formatters.
            return builder.AddMvcOptions(options =>
            {
                var formatter = new ObjectProxyFormatter<TSerialized, TFactory>(factory, options);
                options.OutputFormatters.Insert(0, formatter);
                options.InputFormatters.Insert(0, formatter);
            });
        }

        public static IMvcCoreBuilder AddJsonWebObjectProxy(this IMvcCoreBuilder builder, ISerializationProvider<string> jsonSerializer)
        {
            return AddWebObjectProxy<string, JsonWebObjectProxyFactory>(builder, new JsonWebObjectProxyFactory(jsonSerializer));
        }

        public static IMvcCoreBuilder AddJsonWebObjectProxy(this IMvcCoreBuilder builder, JsonSerializerSettings settings = null)
        {
            return AddWebObjectProxy<string, JsonWebObjectProxyFactory>(builder, new JsonWebObjectProxyFactory(settings));
        }

        public static IMvcCoreBuilder AddJsonWebObjectProxy(this IMvcCoreBuilder builder, Action<JsonSerializerSettings> setupAction)
        {
            return AddWebObjectProxy<string, JsonWebObjectProxyFactory>(builder, new JsonWebObjectProxyFactory(setupAction));
        }
    }
}
