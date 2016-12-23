using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MindSung.HyperState.AspNetCore
{
    internal class ObjectProxyOutputFormatter : IOutputFormatter
    {
        public ObjectProxyOutputFormatter(ObjectProxyFactory<string> factory)
        {
            this.factory = factory;
        }

        ObjectProxyFactory<string> factory;

        public bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (context.ObjectType == null || context.ObjectType.GetTypeInfo().GetGenericTypeDefinition() != typeof(ObjectProxy<,>))
            {
                return false;
            }
            return true;
        }

        public async Task WriteAsync(OutputFormatterWriteContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            var response = context.HttpContext.Response;
            response.ContentType = "application/json";
            using (var writer = context.WriterFactory(response.Body, Encoding.UTF8))
            {
                var proxyInvoker = ObjectProxyInvoker.GetProxyInvoker(context.ObjectType);
                if (proxyInvoker != null)
                {
                    await writer.WriteAsync(proxyInvoker.GetSerialized(context.Object));
                }
                else
                {
                    await writer.WriteAsync(factory.Serializer.Serialize(context.Object));
                }
                await writer.FlushAsync();
            }
        }
    }
}
