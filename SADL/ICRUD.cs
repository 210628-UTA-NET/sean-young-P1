using System;
using System.Linq;

namespace SADL {
    public interface ICRUD<T> where T : SAModels.StoreModel {
        public void Create(T p_model);
        public IQueryable Query(T p_model);
        public void Update(T p_model, string p_idName);
        public void Delete(T p_model, string p_idName);
    }
}
