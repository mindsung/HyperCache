using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindSung.HyperState
{
    public interface IObjectProxy<TObject, TSerialized>
    {
        TSerialized Serialized { get; set; }
        TObject Object { get; set; }
    }

    public interface IObjectProxy<TObject> : IObjectProxy<TObject, string>
    {
    }
}
