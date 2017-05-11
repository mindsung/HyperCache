using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindSung.HyperState
{
    public interface IDualState<TObject, TSerialized>
    {
        TSerialized Serialized { get; set; }
        TObject Object { get; set; }
        void ObjectChanged();
    }

    public interface IDualState<TObject> : IDualState<TObject, string>
    {
    }
}
