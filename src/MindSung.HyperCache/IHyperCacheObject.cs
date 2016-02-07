using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindSung.HyperCache
{
    public interface IHyperCacheObject
    {
        string Key { get; }
    }
}
