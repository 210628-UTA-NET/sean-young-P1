using Microsoft.Extensions.Configuration;
using SADL;
using SAModels;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SABL {
    public class LineItemManager {
        private readonly ICRUD<LineItem> _db;
        private readonly IConfiguration _configuration;

        public LineItemManager(ICRUD<LineItem> p_db, IConfiguration p_configuration) {
            _db = p_db;
            _configuration = p_configuration;
        }

        public LineItem Get(int p_id) {
            IList<Func<LineItem, bool>> conditions = new List<Func<LineItem, bool>>();
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

        public IList<LineItem> QueryProducts(int storefrontId, string p_searchName, string p_category, int p_page) {
            IList<Func<LineItem, bool>> conditions = new List<Func<LineItem, bool>>();
            IList<string> includes = new List<string> {
                "Address",
                "Address.State"
            };

            /*
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

            return _db.Query(new(_configuration) { 
                Conditions = conditions,
                Includes = includes
            });
            */
            return null;
        }
    }
}