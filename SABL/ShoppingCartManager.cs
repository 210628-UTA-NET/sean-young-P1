using Microsoft.Extensions.Configuration;
using SADL;
using SAModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SABL {
    public class ShoppingCartManager {
        private readonly ICRUD<ShoppingCart> _cartDB;
        private readonly IList<string> _includes;
        private readonly IConfiguration _configuration;

        public ShoppingCartManager(ICRUD<ShoppingCart> p_cartDB, IConfiguration p_configuration) {
            _cartDB = p_cartDB;
            _configuration = p_configuration;
            _includes = new List<string>() {
                "Items",
                "Items.Product",
                "Items.Product.Categories"
            };
        }

        public ShoppingCart GetCart(string p_userId, int p_storefrontId) {
            IList<Func<ShoppingCart, bool>> conditions = new List<Func<ShoppingCart, bool>> {
                cart => cart.CustomerUserId == p_userId,
                cart => cart.StorefrontId == p_storefrontId
            };

            ShoppingCart userCart = _cartDB.FindSingle(new(_configuration) {
                Conditions = conditions,
                Includes = _includes
            });


            if (userCart == null) {
                _cartDB.Create(new() { 
                    CustomerUserId = p_userId,
                    StorefrontId = p_storefrontId
                });
                return _cartDB.FindSingle(new(_configuration) {
                    Conditions = conditions
                });
            } else {
                return userCart;
            }
        }
    }
}