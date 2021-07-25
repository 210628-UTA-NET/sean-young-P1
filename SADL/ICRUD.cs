using System;
using System.Collections.Generic;
using System.Linq;

namespace SADL {
    public interface ICRUD<T> where T : class, SAModels.IStoreModel {
        void Create(T p_model);
        IList<T> Query(QueryOptions<T> p_options);
        T FindSingle(QueryOptions<T> p_options);
        void Update(T p_model);
        void Delete(T p_model);
        void Save();
        void FlagForRemoval(T p_model);
    }
}
