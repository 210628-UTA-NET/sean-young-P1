using System;
using System.Linq;

namespace SADL {
    public  interface ICRUD<T> {
        public void Create(T p_model);
        public void Remove(T p_model);
        public void Update(T p_model);
        public void Delete(T p_model);
        public IQueryable Query(T p_model);
    }
}
