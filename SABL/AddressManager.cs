using SADL;
using SAModels;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SABL {
    public class AddressManager {
        private readonly ICRUD<Address> _db;
        public AddressManager(ICRUD<Address> p_db) {
            _db = p_db;
        }
        public Address GetUserAddress(string p_id) {
            List<Func<Address,bool>> conditions = new();
            List<string> includes = new();

            IList<Address> results =_db.Query(conditions, includes);
            return new Address();
        }

        public int Insert(Address p_address) {
            return _db.Create(p_address);
        }

        public int Update(Address p_address) {
            return _db.Update(p_address);
        }
    }
}