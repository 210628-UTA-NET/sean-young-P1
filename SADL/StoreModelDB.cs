using System;
using System.Collections.Generic;
using System.Linq;
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

        public void Delete(T p_model, string p_idName) {
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

        public void Update(T p_model, string p_idName) {
            /*
            var modelID = p_model.GetType().GetProperty(p_idName).GetValue(p_model);

            // Get Item with matching id. Id may not be named "Id"
            T foundModel = _context.Set<T>().Single(e =>
                e.GetType().GetProperty(p_idName).GetValue(e) == modelID);

            // Copy over values and save
            p_model.Transfer(p_model);
            _context.SaveChanges();
            */

            _context.Set<T>().Attach(p_model);
            _context.Set<T>().Update(p_model);
            _context.SaveChanges();

        }
    }
}
