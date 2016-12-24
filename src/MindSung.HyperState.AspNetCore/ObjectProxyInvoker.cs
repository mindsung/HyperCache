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
    internal class ObjectProxyInvoker<TSerialized>
    {
        private ObjectProxyInvoker(Type genericType)
        {
            proxyType = typeof(IObjectProxy<,>).MakeGenericType(genericType, typeof(TSerialized));
            getSerializedMethod = proxyType.GetTypeInfo().GetDeclaredProperty("Serialized").GetMethod;
        }

        Type proxyType;
        MethodInfo getSerializedMethod;

        static ConcurrentDictionary<Type, ObjectProxyInvoker<TSerialized>> proxyInvokers = new ConcurrentDictionary<Type, ObjectProxyInvoker<TSerialized>>();

        public static ObjectProxyInvoker<TSerialized> GetProxyInvoker(Type objectType)
        {
            return proxyInvokers.GetOrAdd(objectType, new ObjectProxyInvoker<TSerialized>(objectType.GetTypeInfo().GenericTypeArguments[0]));
        }

        public string GetSerialized(object proxy)
        {
            return (string)getSerializedMethod.Invoke(proxy, null);
        }
    }
}
