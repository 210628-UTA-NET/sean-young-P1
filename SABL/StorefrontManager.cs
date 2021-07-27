using Microsoft.Extensions.Configuration;
using SADL;
using SAModels;
using System;
using System.Collections.Generic;

namespace SABL {
    /// <summary>
    /// Business Layer class which manages the querying of Storefronts. A user 
    /// can get a single storefront by Id or find storefronts using an address.
    /// </summary>
    public class StorefrontManager {
        private readonly ICRUD<Storefront> _db;
        private readonly IConfiguration _configuration;

        public StorefrontManager(ICRUD<Storefront> p_db, IConfiguration p_configuration) {
            _db = p_db;
            _configuration = p_configuration;
        }

        /// <summary>
        /// Returns a storefront with the given Id. Will return null if
        /// no such storefront exists.
        /// </summary>
        /// <param name="p_id">The id of the storefront to get</param>
        /// <returns></returns>
        public Storefront Get(int p_id) {
            IList<Func<Storefront, bool>> conditions = new List<Func<Storefront, bool>>();
            IList<string> includes = new List<string> {
                "Address",
                "Address.State"
            };
            conditions.Add(sf => sf.Id == p_id);
            return _db.FindSingle(new(_configuration) {
                Conditions = conditions,
                Includes = includes,
            });
        }

        /// <summary>
        /// Queries the database for storefronts with the given address. An 
        /// address may include a zipcode, state, or city. 
        /// </summary>
        /// <param name="p_address">The address to search for Storefronts at</param>
        /// <returns>
        /// A list of storefronts that have an address with either a zipcode,
        /// city or state that the user has specified. An empty list if no 
        /// matches are found.
        /// </returns>
        public IList<Storefront> QueryByAddress(string p_address) {
            IList<Func<Storefront, bool>> conditions = new List<Func<Storefront, bool>>();
            IList<string> includes = new List<string> {
                "Address",
                "Address.State"
            };

            Address testAddress = new();

            try {
                // Check if input string is zipcode
                testAddress.ZipCode = p_address;
                conditions.Add(sf => sf.Address.ZipCode == p_address);
            } catch (FormatException) {
                // Else check by city or state
                conditions.Add(sf =>
                    sf.Address.City.Contains(p_address, StringComparison.OrdinalIgnoreCase) 
                    || sf.Address.State.Code.Contains(p_address, StringComparison.OrdinalIgnoreCase)
                    || sf.Address.State.Name.Contains(p_address, StringComparison.OrdinalIgnoreCase)
                );
            }

            return _db.Query(new(_configuration) { 
                Conditions = conditions,
                Includes = includes
            });
        }
    }
}