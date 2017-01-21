using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindSung.HyperState
{
    public interface IModelProvider<TEntity, TId, TSerialized>
    {
        Task<IObjectProxy<TEntity, TSerialized>> Insert(IObjectProxy<TEntity, TSerialized> value);
        Task<IObjectProxy<TEntity, TSerialized>> Update(TId id, IObjectProxy<TEntity, TSerialized> value);
        Task<IObjectProxy<TEntity, TSerialized>> Get(TId id);
        Task Delete(TId id);
    }
}
