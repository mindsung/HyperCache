using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MindSung.HyperState.AspNetCore
{
    public interface IWebObjectProxyFactory<TSerialized> : IObjectProxyFactory<TSerialized>
    {
        IEnumerable<string> AcceptContentTypes { get; }
        Task<TSerialized> ReadSerialized(HttpRequest request);
        Task WriteSerialized(HttpResponse response, TSerialized serialized);
    }

    public interface IWebObjectProxyFactory : IWebObjectProxyFactory<string>, IObjectProxyFactory
    {
    }
}
