using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindSung.HyperCache.Providers
{
    public interface IStorageProvider<T>
    {
        Task<ObjectProxy<T>> Get(string key);
        Task<IEnumerable<T>> GetAll();
        Task AddOrUpdate(string key, ObjectProxy<T> value);
        Task Remove(string key);
    }

    public interface IStorageProviderFactory
    {
        IStorageProvider<T> Create<T>(string applicationScope, string typeScope);
    }
}
