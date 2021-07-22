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

            //IList<Address> results =_db.Query(new(p_configuration) { Conditions =conditions, includes });
            return new Address();
        }

        public void Insert(Address p_address) {
            _db.Create(p_address);
        }

        public void Update(Address p_address) {
            _db.Update(p_address);
        }
    }
}