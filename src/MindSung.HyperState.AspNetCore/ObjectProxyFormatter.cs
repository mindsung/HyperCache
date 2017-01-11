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
    internal class ObjectProxyFormatter<TSerialized> : IOutputFormatter, IInputFormatter
    {
        public ObjectProxyFormatter(IWebObjectProxyFactory<TSerialized> factory, MvcOptions options)
        {
            this.factory = factory;
            this.options = options;
        }

        private IWebObjectProxyFactory<TSerialized> factory;
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
            return ti.GetInterfaces().Any(ifType => IsInterfaceObjectProxy(ifType));
        }

        bool IsInterfaceObjectProxyEnumerable(Type ifType, out Type objectType)
        {
            if (ifType.GenericTypeArguments.Length == 1 && IsObjectProxy(ifType.GenericTypeArguments[0])
                && (ifType == typeof(IEnumerable<>).MakeGenericType(ifType.GenericTypeArguments[0])
                    || ifType == typeof(IReadOnlyCollection<>).MakeGenericType(ifType.GenericTypeArguments[0])
                    || ifType == typeof(IReadOnlyList<>).MakeGenericType(ifType.GenericTypeArguments[0])))
            {
                objectType = ifType.GenericTypeArguments[0].GenericTypeArguments[0];
                return true;
            }
            else
            {
                objectType = null;
                return false;
            }
        }

        bool IsObjectProxyEnumerable(Type type, bool isInput, out Type objectType)
        {
            objectType = null;
            var ti = type.GetTypeInfo();
            if (ti.IsInterface)
            {
                return IsInterfaceObjectProxyEnumerable(type, out objectType);
            }
            else if (isInput)
            {
                // Input formatting only allow interfaces - IList, ICollection, or IEnumerable.
                return false;
            }
            foreach (var ifType in ti.GetInterfaces())
            {
                if (IsInterfaceObjectProxyEnumerable(ifType, out objectType))
                {
                    return true;
                }
            }
            return false;
        }

        class FromSerializedInvoker
        {
            public FromSerializedInvoker(IWebObjectProxyFactory<TSerialized> factory, Type objectType, bool isCollection)
            {
                this.factory = factory;
                this.isCollection = isCollection;
                if (isCollection)
                {
                    fromSerializedMethod = factory.GetType().GetTypeInfo()
                        .GetMethod("FromSerializedCollection").MakeGenericMethod(objectType);
                }
                else
                {
                    fromSerializedMethod = factory.GetType().GetTypeInfo()
                        .GetMethod("FromSerialized").MakeGenericMethod(objectType);
                }
            }

            bool isCollection;
            IWebObjectProxyFactory<TSerialized> factory;
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
                fromSerializedInvokers.TryAdd(type, new FromSerializedInvoker(factory, type.GetTypeInfo().GenericTypeArguments[0], false));
                return true;
            }
            Type enumerableObjectType;
            if (IsObjectProxyEnumerable(type, true, out enumerableObjectType))
            {
                fromSerializedInvokers.TryAdd(type, new FromSerializedInvoker(factory, enumerableObjectType, true));
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
            public SerializedInvoker(IWebObjectProxyFactory<TSerialized> factory, Type objectType, bool isCollection)
            {
                this.factory = factory;
                this.isCollection = isCollection;
                if (isCollection)
                {
                    getSerializedMethod = factory.GetType().GetTypeInfo()
                        .GetMethod("ToSerializedCollection").MakeGenericMethod(objectType);
                }
                else
                {
                    var proxyType = typeof(IObjectProxy<,>).MakeGenericType(objectType, typeof(TSerialized));
                    getSerializedMethod = proxyType.GetTypeInfo().GetDeclaredProperty("Serialized").GetMethod;
                }
            }

            bool isCollection;
            IWebObjectProxyFactory<TSerialized> factory;
            MethodInfo getSerializedMethod;

            public TSerialized GetSerialized(object proxyOrCollection)
            {
                if (isCollection)
                {
                    return (TSerialized)getSerializedMethod.Invoke(factory, new object[] { proxyOrCollection });
                }
                else
                {
                    return (TSerialized)getSerializedMethod.Invoke(proxyOrCollection, null);
                }
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
                serializedInvokers.TryAdd(type, new SerializedInvoker(factory, type.GetTypeInfo().GenericTypeArguments[0], false));
                return true;
            }
            Type enumerableObjectType;
            if (IsObjectProxyEnumerable(type, false, out enumerableObjectType))
            {
                serializedInvokers.TryAdd(type, new SerializedInvoker(factory, enumerableObjectType, true));
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
