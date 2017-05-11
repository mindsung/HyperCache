using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindSung.HyperState
{
    public class JsonDualState<T> : DualStateBase<T>
    {
        internal JsonDualState(ISerializationProvider<string> serializer)
            : base(serializer)
        {
        }
    }
}
