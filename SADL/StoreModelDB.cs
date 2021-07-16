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
        }

        public void Create(T p_model) {
            _context.Set<T>().Add(p_model);
            _context.SaveChanges();
        }

        public void Delete(T p_model) {
            /*
            var modelID = p_model.GetType().GetProperty(p_idName).GetValue(p_model);

            // Get Item with matching id. Id may not be named "Id"
            T foundModel = _context.Set<T>().Single(e =>
                e.GetType().GetProperty(p_idName).GetValue(e) == modelID);
            */

            // Remove and save
            _context.Set<T>().Attach(p_model);
            _context.Set<T>().Remove(p_model);
            _context.SaveChanges();

        }

        public IQueryable Query(T p_model) {
            throw new NotImplementedException();
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
    }
}
