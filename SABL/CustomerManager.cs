using Microsoft.Extensions.Configuration;
using SADL;
using SAModels;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SABL {
    public class CustomerManager {
        private readonly ICRUD<CustomerUser> _db;
        private readonly IConfiguration _configuration;
        public CustomerManager(ICRUD<CustomerUser> p_db, IConfiguration p_configuration) {
            _db = p_db;
            _configuration = p_configuration;
        }

        public IList<CustomerUser> QueryByName(string p_searchString, int p_page) {
            IList<Func<CustomerUser, bool>> conditions = new List<Func<CustomerUser, bool>>();
            IList<string> includes = new List<string> {
                "Address",
                "Address.State",
            };

            if (p_searchString != null) {
                conditions.Add(sf => (
                    sf.FirstName.Contains(p_searchString, StringComparison.OrdinalIgnoreCase) 
                    || sf.LastName.Contains(p_searchString, StringComparison.OrdinalIgnoreCase)));
            }
            return _db.Query(new(_configuration){ 
                Conditions = conditions,
                Includes = includes,
                Page = p_page
            });
        }
    }
}