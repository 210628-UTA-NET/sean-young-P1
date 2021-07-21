using System;
using System.Collections.Generic;
using System.Linq;

namespace SADL {
    public interface ICRUD<T> where T : class, SAModels.IStoreModel {
        void Create(T p_model);
        IList<T> Query(IList<Func<T, bool>> p_conditions = null, IList<string> p_includes = null, int p_page = 0, int p_pageSize = 0);
        T FindSingle(IList<Func<T, bool>> p_conditions = null, IList<string> p_includes = null);
        void Update(T p_model);
        void Delete(T p_model);
    }
}
