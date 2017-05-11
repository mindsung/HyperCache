using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindSung.HyperState
{
    public abstract class ModelProviderBase<TEntity, TId, TSerialized> : IModelProvider<TEntity, TId, TSerialized>
    {
        public ModelProviderBase<TEntity, TId, TSerialized> DownLevelProvider { get; internal set; }
        public ModelProviderBase<TEntity, TId, TSerialized> UpLevelProvider { get; internal set; }
        public abstract Task Delete(TId id);
        public abstract Task<IDualState<TEntity, TSerialized>> Get(TId id);
        public abstract Task<IDualState<TEntity, TSerialized>> Insert(IDualState<TEntity, TSerialized> value);
        public abstract Task<IDualState<TEntity, TSerialized>> Update(TId id, IDualState<TEntity, TSerialized> value);
    }
}
