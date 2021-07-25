using Microsoft.Extensions.Configuration;
using SADL;
using SAModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SABL {
    public class ShoppingCartManager {
        private readonly ICRUD<ShoppingCart> _cartDb;
        private readonly ICRUD<LineItem> _itemDb;
        private readonly IList<string> _includes;
        private readonly IConfiguration _configuration;

        public ShoppingCartManager(ICRUD<ShoppingCart> p_cartDB, ICRUD<LineItem> p_itemDb, IConfiguration p_configuration) {
            _cartDb = p_cartDB;
            _itemDb = p_itemDb;
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

            ShoppingCart userCart = _cartDb.FindSingle(new(_configuration) {
                Conditions = conditions,
                Includes = _includes
            });


            if (userCart == null) {
                _cartDb.Create(new() { 
                    CustomerUserId = p_userId,
                    StorefrontId = p_storefrontId
                });
                return _cartDb.FindSingle(new(_configuration) {
                    Conditions = conditions
                });
            } else {
                return userCart;
            }
        }

        public void AddItem(int p_itemId, int p_storefrontId, string p_userId, int p_quantity) {
            if (p_quantity < 1) throw new ArgumentException("Cannot add an item with a negative or zero quantity");

            // Load cart associated with user
            ShoppingCart userCart = GetCart(p_userId, p_storefrontId);

            if (userCart.Items == null) {
                userCart.Items = new List<LineItem>();
            }

            // Find the item with the given Id
            LineItem targetItem = _itemDb.FindSingle(new(_configuration) {
                Includes = new List<string>() {
                    "Product",
                    "Product.Categories"
                },
                Conditions = new List<Func<LineItem, bool>> {
                    item => item.Id == p_itemId
                }
            });
            if (targetItem == null) throw new ArgumentException("No item with that ID could be located");

            // Subtract item quantity from target
            targetItem.Quantity -= p_quantity;

            // Check if item with the same productID is in the cart
            LineItem cartItem = userCart.Items.FirstOrDefault(item => item.Product.Id == targetItem.Product.Id);
            if (cartItem != null) {
                cartItem.Quantity += p_quantity;
            } else {
                // Add new item to cart if it does not exist
                userCart.Items.Add(new LineItem() {
                    Product = targetItem.Product,
                    Quantity = p_quantity,
                });
            }

            // Add the price to the cart total
            userCart.TotalAmount += (p_quantity * targetItem.Product.Price);
            _cartDb.Save();
        }

        public void RemoveItem(int p_itemId, int p_storefrontId, string p_userId) {
            ShoppingCart userCart = GetCart(p_userId, p_storefrontId);
            if (userCart == null) throw new ArgumentException("No cart with given IDs could be located");

            // Find the item in the cart
            LineItem targetItem = userCart.Items.FirstOrDefault(item => item.Id == p_itemId);
            if (targetItem == null) throw new ArgumentException("No item with that ID could be located in your cart");

            // Remove item from cart
            foreach (LineItem item in userCart.Items) {
                if (item.Id == p_itemId) {
                    userCart.Items.Remove(item);
                    break;
                }
            }

            // Subtract total price form the cart total
            userCart.TotalAmount -= (targetItem.Quantity * targetItem.Product.Price);


        }
    }
}