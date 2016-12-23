using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace MindSung.HyperState.AspNetCore
{
    public static class MvcCoreExtensions
    {
        public static IMvcCoreBuilder AddHyperState(this IMvcCoreBuilder builder, ObjectProxyFactory<string> factory)
        {
            return builder.AddMvcOptions(options =>
            {
                options.InputFormatters.Insert(0, new ObjectProxyInputFormatter(factory));
                options.OutputFormatters.Insert(0, new ObjectProxyOutputFormatter(factory));
            });
        }

        public static IMvcCoreBuilder AddJsonHyperState(this IMvcCoreBuilder builder, JsonObjectProxyFactory factory)
        {
            return AddHyperState(builder, factory);
        }

        public static IMvcCoreBuilder AddJsonHyperState(this IMvcCoreBuilder builder, Action<JsonSerializerSettings> setupAction = null)
        {
            return AddJsonHyperState(builder, new JsonObjectProxyFactory(setupAction));
        }
    }
}
