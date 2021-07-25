using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SADL {
    public class StoreModelDB<T>: ICRUD<T> where T : class, SAModels.IStoreModel{

        private readonly SADBContext _context;

        public StoreModelDB(SADBContext p_context) {
            _context = p_context;
            //_context.ChangeTracker.LazyLoadingEnabled = false;
        }

        public void Create(T p_model) {
            _context.Set<T>().Add(p_model);
            _context.SaveChanges();
        }

        public void Delete(T p_model) {
            _context.Set<T>().Attach(p_model);
            _context.Set<T>().Remove(p_model);
            _context.SaveChanges();

        }

        public IList<T> Query(QueryOptions<T> p_options) {
            if (p_options.Page < 0 || p_options.PageSize < 0) throw new ArgumentException("Cannot have a negative page size or navigate to a negative page.");

            // Load relations and subrelations
            var queryableQuery = _context.Set<T>().AsQueryable();
            if (p_options.Includes != null) {
                foreach (string inc in p_options.Includes) {
                    queryableQuery = queryableQuery.Include(inc);
                }
            }

            // Add conditions
            var enumerableQuery = queryableQuery.AsEnumerable();
            if (p_options.Conditions != null) {
                foreach (Func<T, bool> cond in p_options.Conditions) {
                    enumerableQuery = enumerableQuery.Where(cond);
                }
            }

            // Paging if applicable
            if (p_options.Paged) {
                int skip = (p_options.Page - 1) * p_options.PageSize;
                enumerableQuery = enumerableQuery.Select(o => o).Skip(skip).Take(p_options.PageSize);
            }

            // Ordering if applicable
            (OrderBy option, string columnName) = p_options.SortOrder;
            if (option == OrderBy.Ascending) {
                enumerableQuery = enumerableQuery.OrderBy(i => typeof(T).GetProperty(columnName).GetValue(i).ToString());
            } else if (option == OrderBy.Descending) {
                enumerableQuery = enumerableQuery.OrderByDescending(i => typeof(T).GetProperty(columnName).GetValue(i).ToString());
            }

            return enumerableQuery.Select(o => o).ToList();
        }

        public T FindSingle(QueryOptions<T> p_options) {
            return Query(p_options).FirstOrDefault();
        }

        public void Update(T p_model) {
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
        }

        public void Save() {
            _context.SaveChanges();
        }
    }
}
