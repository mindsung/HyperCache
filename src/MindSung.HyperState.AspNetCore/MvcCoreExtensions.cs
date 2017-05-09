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

        public static IMvcCoreBuilder AddWebDualState<TSerialized>(this IMvcCoreBuilder builder, IWebDualStateFactory<TSerialized> factory)
        {
            // Always add the factory for the default IWebDualStateFactory interface.
            if (defaultFactories.TryAdd("stringfactory", factory))
            {
                builder.Services.AddSingleton(typeof(IWebDualStateFactory), factory);
            }
            // If this is the first factory added for the TSerialized type, set it as the default factory
            // for dependency injection of the generic type IWebDualStateFactory<TSerialized>
            if (defaultFactories.TryAdd(typeof(TSerialized).FullName, factory))
            {
                builder.Services.AddSingleton(typeof(IWebDualStateFactory<TSerialized>), factory);
            }
            // Add the factory for dependency injection of the specific type.
            builder.Services.AddSingleton(factory.GetType(), factory);
            // Add formatters.
            return builder.AddMvcOptions(options =>
            {
                var formatter = new DualStateFormatter<TSerialized>(factory, options);
                options.OutputFormatters.Insert(0, formatter);
                options.InputFormatters.Insert(0, formatter);
            });
        }

        public static IMvcCoreBuilder AddWebDualState(this IMvcCoreBuilder builder, IWebDualStateFactory factory)
        {
            return AddWebDualState<string>(builder, factory);
        }

        public static IMvcCoreBuilder AddJsonWebDualState(this IMvcCoreBuilder builder, ISerializationProvider<string> jsonSerializer)
        {
            return AddWebDualState(builder, new JsonWebDualStateFactory(jsonSerializer));
        }

        public static IMvcCoreBuilder AddJsonWebDualState(this IMvcCoreBuilder builder, JsonSerializerSettings settings = null)
        {
            return AddWebDualState(builder, new JsonWebDualStateFactory(settings));
        }

        public static IMvcCoreBuilder AddJsonWebDualState(this IMvcCoreBuilder builder, Action<JsonSerializerSettings> setupAction)
        {
            return AddWebDualState(builder, new JsonWebDualStateFactory(setupAction));
        }
    }
}
