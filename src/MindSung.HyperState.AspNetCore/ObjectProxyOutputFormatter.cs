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
    internal class ObjectProxyOutputFormatter<TSerialized, TFactory> : IOutputFormatter
        where TFactory : IWebObjectProxyFactory<TSerialized>
    {
        public ObjectProxyOutputFormatter(TFactory factory, MvcOptions options)
        {
            this.factory = factory;
            this.options = options;
        }

        TFactory factory;
        MvcOptions options;

        private bool IsObjectProxy(Type type)
        {
            var ifs = type.GetTypeInfo().GetGenericTypeDefinition().GetInterfaces();
            return ifs.Any(i =>
            {
                if (i.GenericTypeArguments.Length != 2)
                {
                    return false;
                }
                var ifop = typeof(IObjectProxy<,>).MakeGenericType(i.GenericTypeArguments[0], typeof(TSerialized));
                return i == ifop;
            });
        }

        public bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            // We'll format the object if it is an object proxy or if no other registered formatters will handle it.
            return context.ObjectType != null && (IsObjectProxy(context.ObjectType)
                || !options.OutputFormatters.Where(f => f.GetType() != GetType()).Any(f => f.CanWriteResult(context)));
        }

        public async Task WriteAsync(OutputFormatterWriteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            var response = context.HttpContext.Response;
            response.ContentType = factory.OutputContentType;
            using (var writer = context.WriterFactory(response.Body, Encoding.UTF8))
            {
                if (context.ObjectType != null)
                {
                    if (IsObjectProxy(context.ObjectType))
                    {
                        await writer.WriteAsync(ObjectProxyInvoker<TSerialized>.GetProxyInvoker(context.ObjectType).GetSerialized(context.Object));
                    }
                    else
                    {
                        await factory.WriteSerialized(writer, factory.Serializer.Serialize(context.Object, context.ObjectType));
                    }
                }
                await writer.FlushAsync();
            }
        }
    }
}
