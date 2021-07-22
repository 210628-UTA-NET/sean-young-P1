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

        public IList<LineItem> QueryInventory(int storefrontId, string p_searchName, string p_category, int p_page) {
            IList<Func<LineItem, bool>> conditions = new List<Func<LineItem, bool>>();
            IList<string> includes = new List<string> {
                "Product",
            };

            conditions.Add(sf => sf.Id == storefrontId);

            return _db.Query(new(_configuration) {
                Conditions = conditions,
                Includes = includes,
            });
        }
    }
}