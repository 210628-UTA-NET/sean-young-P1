using System;
using System.Collections.Generic;
using System.Linq;

namespace SADL {
    public interface ICRUD<T> where T : SAModels.StoreModel {
        void Create(T p_model);
        IList<T> Query(IList<Func<T, bool>> p_conditions = null, IList<string> p_includes = null, int p_page = 0, int p_pageSize = 0);
        T FindByID(int p_id);
        T FindByID(string p_id);
        void Update(T p_model);
        void Delete(T p_model);
    }
}
