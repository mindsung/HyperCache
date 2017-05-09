using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindSung.HyperState
{
    public interface IModelProvider<TEntity, TId, TSerialized>
    {
        Task<IDualState<TEntity, TSerialized>> Insert(IDualState<TEntity, TSerialized> value);
        Task<IDualState<TEntity, TSerialized>> Update(TId id, IDualState<TEntity, TSerialized> value);
        Task<IDualState<TEntity, TSerialized>> Get(TId id);
        Task Delete(TId id);
    }
}
