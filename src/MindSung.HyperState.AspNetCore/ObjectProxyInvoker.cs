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
    //internal class ObjectProxyInvoker
    //{
    //    private ObjectProxyInvoker(Type genericType)
    //    {
    //        proxyType = typeof(ObjectProxy<>).MakeGenericType(genericType);
    //        getSerializedMethod = proxyType.GetTypeInfo().GetDeclaredMethod("GetSerialized");
    //        setSerializedMethod = proxyType.GetTypeInfo().GetDeclaredMethod("SetSerialized");
    //    }

    //    Type proxyType;
    //    MethodInfo getSerializedMethod;
    //    MethodInfo setSerializedMethod;

    //    public string GetSerialized(object proxy)
    //    {
    //        return (string)getSerializedMethod.Invoke(proxy, null);
    //    }

    //    public object SetSerialized(string serialized)
    //    {
    //        var proxy = Activator.CreateInstance(proxyType);
    //        setSerializedMethod.Invoke(proxy, new object[] { serialized });
    //        return proxy;
    //    }

    //    static ConcurrentDictionary<Type, ObjectProxyInvoker> proxyInvokers = new ConcurrentDictionary<Type, ObjectProxyInvoker>();

    //    public static ObjectProxyInvoker GetProxyInvoker(Type objectType)
    //    {
    //        ObjectProxyInvoker proxyInvoker;
    //        if (proxyInvokers.TryGetValue(objectType, out proxyInvoker))
    //        {
    //            return proxyInvoker;
    //        }
    //        var typeInfo = objectType.GetTypeInfo();
    //        var tProxy = typeof(ObjectProxy<>);
    //        if (typeInfo.GetGenericTypeDefinition() == tProxy)
    //        {
    //            proxyInvoker = new ObjectProxyInvoker(typeInfo.GenericTypeArguments[0]);
    //            proxyInvokers[objectType] = proxyInvoker;
    //            return proxyInvoker;
    //        }
    //        else
    //        {
    //            return null;
    //        }
    //    }
    //}
}
