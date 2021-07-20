using Microsoft.Extensions.Configuration;
using SADL;
using SAModels;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SABL {
    public class StorefrontManager {
        private readonly ICRUD<Storefront> _db;
        private readonly IConfiguration _configuration;

        public StorefrontManager(ICRUD<Storefront> p_db, IConfiguration p_configuration) {
            _db = p_db;
            _configuration = p_configuration;
        }

        public IList<Storefront> QueryByAddress(string p_address, int p_page) {
            var pageSizeOptions = new SAOptions();
            _configuration.GetSection(SAOptions.PageOptions).Bind(pageSizeOptions);

            IList<Func<Storefront, bool>> conditions = new List<Func<Storefront, bool>>();
            IList<string> includes = new List<string> {
                "Address",
                "Address.State"
            };

            Address testAddress = new();

            try {
                // Check if input string is zipcode
                testAddress.ZipCode = p_address;
                conditions.Add(sf => sf.Address.ZipCode == testAddress.ZipCode);
            } catch (ArgumentException) {
                // Else check by city or state
                conditions.Add(sf =>
                    p_address.Contains(sf.Address.City) 
                    || p_address.Contains(sf.Address.State.Code)
                    || p_address.Contains(sf.Address.State.Name)
                );
            }

            return _db.Query(conditions, includes, p_page, pageSizeOptions.PageSize);
        }
    }
}