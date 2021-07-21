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
        private readonly SAOptions _pageSizeOptions;

        public StorefrontManager(ICRUD<Storefront> p_db, IConfiguration p_configuration) {
            _db = p_db;
            _pageSizeOptions = new SAOptions();
            p_configuration.GetSection(SAOptions.PageOptions).Bind(_pageSizeOptions);
        }

        public Storefront Get(int p_id) {
            IList<Func<Storefront, bool>> conditions = new List<Func<Storefront, bool>>();
            IList<string> includes = new List<string> {
                "Address",
                "Address.State"
            };
            conditions.Add(sf => sf.Id == p_id);
            return _db.FindSingle(conditions, includes);
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
                conditions.Add(sf => sf.Address.ZipCode == p_address);
            } catch (FormatException) {
                // Else check by city or state
                conditions.Add(sf =>
                    p_address.Contains(sf.Address.City, StringComparison.OrdinalIgnoreCase) 
                    || p_address.Contains(sf.Address.State.Code, StringComparison.OrdinalIgnoreCase)
                    || p_address.Contains(sf.Address.State.Name, StringComparison.OrdinalIgnoreCase)
                );
            }

            return _db.Query(conditions, includes, p_page, pageSizeOptions.PageSize);
        }
    }
}