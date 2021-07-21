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
        private readonly SAOptions _pageSizeOptions;
        public CustomerManager(ICRUD<CustomerUser> p_db, IConfiguration p_configuration) {
            _db = p_db;
            _pageSizeOptions = new SAOptions();
            p_configuration.GetSection(SAOptions.PageOptions).Bind(_pageSizeOptions);
        }

        public IList<CustomerUser> QueryByName(string p_searchString, int p_page) {
            IList<Func<CustomerUser, bool>> conditions = new List<Func<CustomerUser, bool>>();
            IList<string> includes = new List<string> {
                "Address",
                "Address.State",
            };

            conditions.Add(sf => (sf.FirstName.Contains(p_searchString) || sf.LastName.Contains(p_searchString)));

            return _db.Query(conditions, includes, p_page, _pageSizeOptions.PageSize);
        }
    }
}