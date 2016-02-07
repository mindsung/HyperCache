using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace MindSung.HyperCache.Providers
{
    public class LocalMemoryCacheProviderFactory : ICacheProviderFactory
    {
        class LocalMemoryCacheProvider : ConcurrentDictionary<string, string>, ICacheProvider
        {
            public Task<string> Get(string key)
            {
                string serialized = null;
                TryGetValue(key, out serialized);
                return Task.FromResult(serialized);
            }

            public Task AddOrUpdate(string key, string serialized)
            {
                this[key] = serialized;
                return Task.FromResult(true);
            }

            public Task Remove(string key)
            {
                string _;
                TryRemove(key, out _);
                return Task.FromResult(true);
            }
        }

        public ICacheProvider Create(string applicationScope, string typeScope)
        {
            return new LocalMemoryCacheProvider();
        }
    }
}
