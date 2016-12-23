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
    //public class ObjectProxyInputFormatter : IInputFormatter
    //{
    //    public ObjectProxyInputFormatter(ObjectProxyFactory<string> factory)
    //    {
    //        this.factory = factory;
    //    }

    //    ObjectProxyFactory<string> factory;

    //    public bool CanRead(InputFormatterContext context)
    //    {
    //        if (context == null)
    //            throw new ArgumentNullException(nameof(context));
    //        var contentType = context.HttpContext.Request.ContentType;
    //        if (contentType == null
    //            || contentType == "application/json")
    //            return true;
    //        return false;
    //    }
    //    public async Task<InputFormatterResult> ReadAsync(InputFormatterContext context)
    //    {
    //        if (context == null)
    //            throw new ArgumentNullException(nameof(context));
    //        var request = context.HttpContext.Request;
    //        if (request.ContentLength == 0)
    //        {
    //            if (context.ModelType.GetTypeInfo().IsValueType)
    //                return await InputFormatterResult.SuccessAsync(Activator.CreateInstance(context.ModelType));
    //            else return await InputFormatterResult.SuccessAsync(null);
    //        }
    //        var encoding = Encoding.UTF8;//do we need to get this from the request im not sure yet
    //        using (var reader = new StreamReader(context.HttpContext.Request.Body))
    //        {
    //            var proxyInvoker = ObjectProxyInvoker.GetProxyInvoker(context.ModelType);
    //            if (proxyInvoker != null)
    //            {
    //                var proxy = proxyInvoker.SetSerialized(await reader.ReadToEndAsync());
    //                return await InputFormatterResult.SuccessAsync(proxy);
    //            }
    //            else
    //            {
    //                var model = serializer.Deserialize(reader, context.ModelType);
    //                return await InputFormatterResult.SuccessAsync(model);
    //            }
    //        }
    //    }
    //}
}
