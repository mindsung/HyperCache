using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindSung.HyperState
{
    public class HyperStateModel<TEntity, TId, TSerialized> : IModelProvider<TEntity, TId, TSerialized>
    {
        public HyperStateModel(params ModelProviderBase<TEntity, TId, TSerialized>[] modelProviders)
        {
            for (var i = 0; i < modelProviders.Length; i++)
            {
                if (i > 0) { modelProviders[i - 1].UpLevelProvider = modelProviders[i]; }
                if (i < (modelProviders.Length - 1)) { modelProviders[i + 1].DownLevelProvider = modelProviders[i]; }
            }
            this.modelProviders = modelProviders;
        }

        private ModelProviderBase<TEntity, TId, TSerialized>[] modelProviders;

        public async Task<IObjectProxy<TEntity, TSerialized>> Insert(IObjectProxy<TEntity, TSerialized> value)
        {
            IObjectProxy<TEntity, TSerialized> propagateValue = value;
            foreach (var model in modelProviders)
            {
                propagateValue = await model.Insert(propagateValue);
            }
            return propagateValue;
        }

        public async Task<IObjectProxy<TEntity, TSerialized>> Update(TId id, IObjectProxy<TEntity, TSerialized> value)
        {
            IObjectProxy<TEntity, TSerialized> propagateValue = value;
            foreach (var model in modelProviders)
            {
                propagateValue = await model.Update(id, propagateValue);
            }
            return propagateValue;
        }

        public async Task<IObjectProxy<TEntity, TSerialized>> Get(TId id)
        {
            IObjectProxy<TEntity, TSerialized> value;
            foreach (var model in modelProviders.Reverse())
            {
                value = await model.Get(id);
                if (value != null) { return value; }
            }
            return null;
        }

        public async Task Delete(TId id)
        {
            foreach (var model in modelProviders)
            {
                await model.Delete(id);
            }
        }
    }
}
