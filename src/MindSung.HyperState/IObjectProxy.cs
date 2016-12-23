using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindSung.HyperState
{
    public interface IObjectProxy<TObject, TSerialized> where TObject : class where TSerialized : class
    {
        TSerialized GetSerialized();
        void SetSerialized(TSerialized serialized);
        TObject GetObject();
        void SetObject(TObject value);
    }
}
