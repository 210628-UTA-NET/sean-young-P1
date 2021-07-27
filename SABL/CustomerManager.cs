using Microsoft.Extensions.Configuration;
using SADL;
using SAModels;
using System;
using System.Collections.Generic;

namespace SABL {
    /// <summary>
    /// Business Layer class whcih manages the querying of customers by the name
    /// parameter. 
    /// </summary>
    public class CustomerManager {
        private readonly ICRUD<CustomerUser> _db;
        private readonly IConfiguration _configuration;


        /// <param name="p_db">Data Layer interface that can perform CRUD operations on CustomerUsers</param>
        /// <param name="p_configuration">Configurations from configuration file</param>
        public CustomerManager(ICRUD<CustomerUser> p_db, IConfiguration p_configuration) {
            _db = p_db;
            _configuration = p_configuration;
        }

        /// <summary>
        /// Queries the database for CustomerUsers with first or last names which
        /// contain the given search string. Case insensitive.
        /// </summary>
        /// <param name="p_searchString">The pattern to search for in the names</param>
        /// <param name="p_page">The index of the page if the results are paged</param>
        /// <returns>
        /// A list of CustomerUsers with a first or last name containing the 
        /// search string. Will return an empty list if no matches are found.
        /// </returns>
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