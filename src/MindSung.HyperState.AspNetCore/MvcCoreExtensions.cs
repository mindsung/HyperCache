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
        public static IMvcCoreBuilder AddHyperStateJsonFormatters(this IMvcCoreBuilder builder, Action<JsonSerializerSettings> setupAction = null)
        {
            var jsonSettings = JsonConvert.DefaultSettings?.Invoke() ?? new JsonSerializerSettings();
            setupAction?.Invoke(jsonSettings);

            return builder.AddMvcOptions(options =>
            {
                options.InputFormatters.Insert(0, new ObjectProxyInputFormatter(jsonSettings));
                options.OutputFormatters.Insert(0, new ObjectProxyOutputFormatter(jsonSettings));
            })
            .AddJsonFormatters(setupAction ?? (_ => { }));
        }
    }
}
