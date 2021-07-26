using Microsoft.Extensions.Configuration;
using SADL;
using SAModels;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SABL {
    public class OrderManager {
        private readonly ICRUD<Order> _db;
        private readonly IConfiguration _configuration;
        private readonly IList<string> _includes;
        public OrderManager(ICRUD<Order> p_db, IConfiguration p_configuration) {
            _db = p_db;
            _configuration = p_configuration;
            _includes = new List<string>() {
                "LineItems",
                "LineItems.Product",
                "Storefront"
            };
        }

        public IList<Order> QueryById(string p_id) {
            IList<Func<Order, bool>> conditions = new List<Func<Order, bool>> {
                o => o.CustomerUserId == p_id
            };
            return _db.Query(new(_configuration){ 
                Conditions = conditions,
                Includes = _includes
            });
        }

        public IList<Order> QueryById(int p_id) {
            IList<Func<Order, bool>> conditions = new List<Func<Order, bool>> {
                o => o.StorefrontId == p_id
            };
            return _db.Query(new(_configuration) {
                Conditions = conditions,
                Includes = _includes
            });
        }
    }
}