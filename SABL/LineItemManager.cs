using Microsoft.Extensions.Configuration;
using SADL;
using SAModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SABL {
    /// <summary>
    /// Business Layer class which manages the querying of LineItems, related
    /// entities, and modifying the quanity of LineItems.
    /// </summary>
    public class LineItemManager {
        private readonly ICRUD<LineItem> _lineItemDb;
        private readonly ICRUD<Category> _categoryDb;
        private readonly IConfiguration _configuration;
        private readonly IList<string> _includes;

        /// <param name="p_lineItemDb">Data Layer interface that can perform CRUD operations on LineItems</param>
        /// <param name="p_categoryDb">Data Layer interface that can perform CRUD operations on Categories</param>
        /// <param name="p_configuration">Configurations from configuration file</param>
        public LineItemManager(ICRUD<LineItem> p_lineItemDb, ICRUD<Category> p_categoryDb, IConfiguration p_configuration) {
            _lineItemDb = p_lineItemDb;
            _categoryDb = p_categoryDb;
            _configuration = p_configuration;
            _includes = new List<string>() {
                "Product",
                "Product.Categories"
            };
        }

        /// <summary>
        /// Searches the database for all LineItems with the given search name 
        /// and/or category. If either field is null then it will be ignored
        /// as a search parameter. If both fields are null then all LineItems
        /// at the given storefront will be returned.
        /// </summary>
        /// <param name="p_storefrontId">
        /// The Id of the storefront whose inventory will be searched (required).
        /// </param>
        /// <param name="p_searchName">
        /// The name of the product(s) to search for
        /// </param>
        /// <param name="p_category">
        /// The category of the product(s) to search for
        /// </param>
        /// <returns></returns>
        public virtual IList<LineItem> QueryStoreInventory(int p_storefrontId, string p_searchName, string p_category) {
            IList<Func<LineItem, bool>> conditions = new List<Func<LineItem, bool>> {
                item => item.StorefrontId == p_storefrontId
            };

            if (p_category != null) {
                conditions.Add(item => {
                    IEnumerable<string> categories = item.Product.Categories.Select(c => c.Name);
                    return categories.Contains(p_category);
                });
            }

            if (p_searchName != null) {
                conditions.Add(item =>
                    item.Product.Name.Contains(p_searchName, StringComparison.OrdinalIgnoreCase)
                );
            }

            return _lineItemDb.Query(new(_configuration) {
                Conditions = conditions,
                Includes = _includes,
            });
        }

        /// <summary>
        /// Adds the given quantity to the LineItem with the given Id. Negative
        /// values can be added but the quanity of a LineItem cannot be less
        /// than zero.
        /// </summary>
        /// <param name="p_id">The Id of the LineItem to replenish</param>
        /// <param name="p_quantity">The quantity to add to the item</param>
        public void ReplenishItem(int p_id, int p_quantity) {
            IList<Func<LineItem, bool>> conditions = new List<Func<LineItem, bool>> {
                item => item.Id == p_id
            };
            LineItem updateItem = _lineItemDb.FindSingle(new(_configuration){
                Conditions = conditions
            });
            if (updateItem == null) throw new ArgumentException("Item with ID not found.");
            if (updateItem.Quantity + p_quantity < 0) throw new ArgumentException("Cannot have a negative quantity.");

            updateItem.Quantity += p_quantity;
            _lineItemDb.Save();
        }

        /// <summary>
        /// Returns all the Categories of all the products at the specified
        /// storefront
        /// </summary>
        /// <param name="p_storefrontId"> 
        /// The Id of the storefront from whose products the categories will be
        /// generated.
        /// </param>
        /// <returns>A list of all the categories</returns>
        public IList<Category> GetCategories(int p_storefrontId) {
            //return _categoryDb.Query(new(null)).OrderBy(c => c.Name).ToList();

            // Get ids of all products at a storefront
           var productIds = _lineItemDb.Query(new(_configuration) {
                Includes = _includes,
                Conditions = new List<Func<LineItem, bool>>() {
                    item => item.StorefrontId == p_storefrontId,
                }
            }).Select(item => item.Product.Id);

            IList<Category> results = _categoryDb.Query(new(null){
                Includes = new List<string>() {"Products"},
                Conditions = new List<Func<Category, bool>>() {
                    c => c.Products.Select(p => p.Id).Intersect(productIds).Any()
                }
            });

            return results;
        }
    }
}