using System;
using System.Collections.Generic;
using System.Linq;

namespace SADL {
    public interface ICRUD<T> where T : SAModels.StoreModel {
        void Create(T p_model);
        IList<T> Query(IList<Func<T, bool>> p_conditions, IList<string> p_includes);
        void Update(T p_model);
        void Delete(T p_model);
    }
}
