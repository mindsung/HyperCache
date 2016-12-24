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
        //public static IMvcCoreBuilder AddHyperState(this IMvcCoreBuilder builder, ObjectProxyFactory<string> factory)
        //{
        //    return builder.AddMvcOptions(options =>
        //    {
        //        options.InputFormatters.Insert(0, new ObjectProxyInputFormatter(factory));
        //        options.OutputFormatters.Insert(0, new ObjectProxyOutputFormatter(factory));
        //    });
        //}

        //public static IMvcCoreBuilder AddJsonHyperState(this IMvcCoreBuilder builder, JsonObjectProxyFactory factory)
        //{
        //    return AddHyperState(builder, factory);
        //}

        //public static IMvcCoreBuilder AddJsonHyperState(this IMvcCoreBuilder builder, Action<JsonSerializerSettings> setupAction = null)
        //{
        //    return AddJsonHyperState(builder, new JsonObjectProxyFactory(setupAction));
        //}

        private static ConcurrentDictionary<Type, object> defaultFactories = new ConcurrentDictionary<Type, object>();

        public static IMvcCoreBuilder AddWebObjectProxy<TSerialized>(this IMvcCoreBuilder builder, IWebObjectProxyFactory<TSerialized> factory)
        {
            // If this is the first factory added for the TSerialized type, set it as the default factory
            // for dependency injection of the generic type IWebObjectProxyFactory<TSerialized>
            if (defaultFactories.TryAdd(typeof(TSerialized), factory))
            {
                builder.Services.AddTransient(typeof(IWebObjectProxyFactory<TSerialized>), _ => defaultFactories[typeof(TSerialized)]);
            }
            // Add the factory for dependency injection of the specific type.
            builder.Services.AddTransient(factory.GetType(), _ => factory);
            // TODO: Add Input/Output formatters.
            return builder;
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
