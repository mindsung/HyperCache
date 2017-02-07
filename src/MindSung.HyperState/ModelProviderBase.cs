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
        public abstract Task<IObjectProxy<TEntity, TSerialized>> Get(TId id);
        public abstract Task<IObjectProxy<TEntity, TSerialized>> Insert(IObjectProxy<TEntity, TSerialized> value);
        public abstract Task<IObjectProxy<TEntity, TSerialized>> Update(TId id, IObjectProxy<TEntity, TSerialized> value);
    }
}
