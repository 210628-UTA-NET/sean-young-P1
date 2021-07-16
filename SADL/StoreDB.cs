using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SADL {
    public class CustomerDB : ICRUD<SAModels.Customer>{

        private readonly SADBContext _context;

        public CustomerDB(SADBContext p_context) {
            _context = p_context;
        }

        public void Create(SAModels.Customer p_model) {
            throw new NotImplementedException();
        }

        public void Delete(SAModels.Customer p_model) {
            throw new NotImplementedException();
        }

        public IQueryable Query(SAModels.Customer p_model){
            throw new NotImplementedException();
        }

        public void Remove(SAModels.Customer p_model) {
            throw new NotImplementedException();
        }

        public void Update(SAModels.Customer p_model) {
            throw new NotImplementedException();
        }
    }
}
