using Microsoft.Extensions.Configuration;
using SADL;
using SAModels;
using System;
using System.Collections.Generic;

namespace SABL {
    /// <summary>
    /// Business Layer class which manages the querying of Orders by Id. A user
    /// can query orders by userId or storefrontId.
    /// </summary>
    public class OrderManager {
        private readonly ICRUD<Order> _db;
        private readonly IConfiguration _configuration;
        private readonly IList<string> _includes;


        /// <param name="p_db">Data Layer interface that can perform CRUD operations on Orders</param>
        /// <param name="p_configuration">Configurations from configuration file</param>
        public OrderManager(ICRUD<Order> p_db, IConfiguration p_configuration) {
            _db = p_db;
            _configuration = p_configuration;
            _includes = new List<string>() {
                "LineItems",
                "LineItems.Product",
                "Storefront",
                "CustomerUser"
            };
        }

        /// <summary>
        /// Queries the database for Orders with the given user Id.
        /// </summary>
        /// <param name="p_id">The Id to query orders for</param>
        /// <returns>A list of Orders associated with the given Id</returns>
        public IList<Order> QueryById(string p_id) {
            IList<Func<Order, bool>> conditions = new List<Func<Order, bool>> {
                o => o.CustomerUserId == p_id
            };
            return _db.Query(new(_configuration){ 
                Conditions = conditions,
                Includes = _includes
            });
        }

        /// <summary>
        /// Queries the database for Orders with the given storefront Id.
        /// </summary>
        /// <param name="p_id">The Id to query orders for</param>
        /// <returns>A list of Orders associated with the given Id</returns>
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