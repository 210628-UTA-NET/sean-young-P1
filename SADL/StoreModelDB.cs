using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SADL {
    public class StoreModelDB<T>: ICRUD<T> where T : SAModels.StoreModel {

        private readonly SADBContext _context;

        public StoreModelDB(SADBContext p_context) {
            _context = p_context;
            //_context.ChangeTracker.LazyLoadingEnabled = false;
        }

        public int Create(T p_model) {
            _context.Set<T>().Add(p_model);
            _context.SaveChanges();
            return p_model.GetID();
        }

        public void Delete(T p_model) {
            _context.Set<T>().Attach(p_model);
            _context.Set<T>().Remove(p_model);
            _context.SaveChanges();

        }

        public IList<T> Query(IList<Func<T, bool>> p_conditions, IList<string> p_includes) {
            var enumerableQuery = _context.Set<T>().AsEnumerable();

            if (p_conditions != null) {
                foreach (Func<T, bool> cond in p_conditions) {
                    enumerableQuery = enumerableQuery.Where(cond);
                }
            }

            var queryableQuery = enumerableQuery.AsQueryable();

            if (p_includes != null) {
                foreach (string inc in p_includes) {
                    queryableQuery = queryableQuery.Include(inc);
                }
            }

            return queryableQuery.Select(o => o).ToList();
        }

        public T FindByID(int p_id) {
            return _context.Set<T>().Find(p_id);
        }

        public int Update(T p_model) {
            _context.Set<T>().Attach(p_model);

            var entry = _context.Entry(p_model);
            entry.State = EntityState.Modified;

            foreach(var prop in entry.Properties.Where(p => !p.Metadata.IsKey()) ) {
                if (prop.CurrentValue == null) {
                    prop.IsModified = false;
                } 
            }

            _context.SaveChanges();
            entry.State = EntityState.Detached;
            return p_model.GetID();
        }
    }
}
