using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindSung.HyperCache.Providers
{
    public interface ICacheProvider
    {
        Task<string> Get(string key);
        Task AddOrUpdate(string key, string serialized);
        Task Remove(string key);
    }

    public interface ICacheProviderFactory
    {
        ICacheProvider Create(string applicationScope, string typeScope);
    }
}
