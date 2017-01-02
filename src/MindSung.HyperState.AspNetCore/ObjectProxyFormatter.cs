using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Http;
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
    internal class ObjectProxyFormatter<TSerialized, TFactory> : IOutputFormatter, IInputFormatter
        where TFactory : IWebObjectProxyFactory<TSerialized>
    {
        public ObjectProxyFormatter(TFactory factory, MvcOptions options)
        {
            this.factory = factory;
            this.options = options;
        }

        private TFactory factory;
        private MvcOptions options;

        bool IsInterfaceObjectProxy(Type ifType)
        {
            return ifType.GenericTypeArguments.Length == 2 && ifType.GenericTypeArguments[1] == typeof(TSerialized)
                    && ifType == typeof(IObjectProxy<,>).MakeGenericType(ifType.GenericTypeArguments[0], typeof(TSerialized));
        }

        bool IsObjectProxy(Type type)
        {
            var ti = type.GetTypeInfo();
            if (ti.IsInterface)
            {
                return IsInterfaceObjectProxy(type);
            }
            return ti.GetInterfaces().Any(i => IsInterfaceObjectProxy(i));
        }

        class FromSerializedInvoker
        {
            public FromSerializedInvoker(TFactory factory, Type objectType)
            {
                this.factory = factory;
                fromSerializedMethod = factory.GetType().GetTypeInfo()
                    .GetMethod("FromSerialized").MakeGenericMethod(objectType);
            }

            TFactory factory;
            MethodInfo fromSerializedMethod;

            public object FromSerialized(TSerialized serialized)
            {
                return fromSerializedMethod.Invoke(factory, new object[] { serialized });
            }
        }

        private ConcurrentDictionary<Type, FromSerializedInvoker> fromSerializedInvokers = new ConcurrentDictionary<Type, FromSerializedInvoker>();

        public bool CanRead(InputFormatterContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            var contentType = context.HttpContext.Request.GetTypedHeaders().ContentType?.MediaType;
            if (contentType == null || !factory.AcceptContentTypes.Contains(contentType))
            {
                return false;
            }
            var type = context.ModelType;
            // We'll format the object if it is an object proxy or if no other registered formatters will handle it.
            if (IsObjectProxy(type))
            {
                fromSerializedInvokers.TryAdd(type, new FromSerializedInvoker(factory, type.GetTypeInfo().GenericTypeArguments[0]));
                return true;
            }
            if (!options.InputFormatters.Where(f => f.GetType() != GetType()).Any(f => f.CanRead(context)))
            {
                return true;
            }
            return false;
        }

        public async Task<InputFormatterResult> ReadAsync(InputFormatterContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            var serialized = await factory.ReadSerialized(context.HttpContext.Request);
            FromSerializedInvoker invoker;
            if (fromSerializedInvokers.TryGetValue(context.ModelType, out invoker))
            {
                return InputFormatterResult.Success(invoker.FromSerialized(serialized));
            }
            else
            {
                return InputFormatterResult.Success(factory.Serializer.Deserialize(serialized, context.ModelType));
            }
        }

        class SerializedInvoker
        {
            public SerializedInvoker(Type objectType)
            {
                var proxyType = typeof(IObjectProxy<,>).MakeGenericType(objectType, typeof(TSerialized));
                getSerializedMethod = proxyType.GetTypeInfo().GetDeclaredProperty("Serialized").GetMethod;
            }

            MethodInfo getSerializedMethod;

            public TSerialized GetSerialized(object proxy)
            {
                return (TSerialized)getSerializedMethod.Invoke(proxy, null);
            }
        }

        private ConcurrentDictionary<Type, SerializedInvoker> serializedInvokers = new ConcurrentDictionary<Type, SerializedInvoker>();

        public bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            var type = context.ObjectType;
            if (type == null)
            {
                return false;
            }
            // We'll format the object if it is an object proxy or if no other registered formatters will handle it.
            if (IsObjectProxy(type))
            {
                serializedInvokers.TryAdd(type, new SerializedInvoker(type.GetTypeInfo().GenericTypeArguments[0]));
                return true;
            }
            if (!options.OutputFormatters.Where(f => f.GetType() != GetType()).Any(f => f.CanWriteResult(context)))
            {
                return true;
            }
            return false;
        }

        public async Task WriteAsync(OutputFormatterWriteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (context.ObjectType == null)
            {
                throw new ArgumentNullException(nameof(context.ObjectType));
            }
            TSerialized serialized;
            SerializedInvoker invoker;
            if (serializedInvokers.TryGetValue(context.ObjectType, out invoker))
            {
                serialized = invoker.GetSerialized(context.Object);
            }
            else
            {
                serialized = factory.Serializer.Serialize(context.Object, context.ObjectType);
            }
            await factory.WriteSerialized(context.HttpContext.Response, serialized);
        }
    }
}
