using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MindSung.HyperState.AspNetCore
{
    public interface IWebObjectProxyFactory<TSerialized> : IObjectProxyFactory<TSerialized>
    {
        Type GetObjectProxyType<TObject>();
        IEnumerable<string> AcceptContectTypes { get; }
        string OutputContentType { get; }
        Task<TSerialized> ReadAsSerialized(TextReader reader);
        Task WriteSerialized(TextWriter writer, TSerialized serialized);
    }
}
