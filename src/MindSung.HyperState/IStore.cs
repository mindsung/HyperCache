using System;
using System.Threading.Tasks;

namespace MindSung.HyperState
{
    public interface IStore<TKey, TValue>
    {
        Task<TValue> Get(TKey key);
        Task Put(TKey key, TValue value);
        Task Delete(TKey key);
    }
}
