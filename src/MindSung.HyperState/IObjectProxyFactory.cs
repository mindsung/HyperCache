using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindSung.HyperState
{
    public interface IObjectProxyFactory<TSerialized>
    {
        IObjectProxy<TObject, TSerialized> FromObject<TObject>(TObject obj);
        IObjectProxy<TObject, TSerialized> FromSerialized<TObject>(TSerialized serialized);
    }
}
