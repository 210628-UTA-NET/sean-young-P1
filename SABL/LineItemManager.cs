using Microsoft.Extensions.Configuration;
using SADL;
using SAModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SABL {
    public class LineItemManager {
        private readonly ICRUD<LineItem> _lineItemDb;
        private readonly ICRUD<Category> _categoryDb;
        private readonly IConfiguration _configuration;
        private readonly IList<string> _includes;

        public LineItemManager(ICRUD<LineItem> p_lineItemDb, ICRUD<Category> p_categoryDb, IConfiguration p_configuration) {
            _lineItemDb = p_lineItemDb;
            _categoryDb = p_categoryDb;
            _configuration = p_configuration;
            _includes = new List<string>() {
                "Product",
                "Product.Categories"
            };
        }

        public IList<LineItem> QueryStoreInventory(int p_storefrontId, string p_searchName, string p_category) {
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

        public void ReplenishItem(int p_id, int p_quantity) {
            IList<Func<LineItem, bool>> conditions = new List<Func<LineItem, bool>> {
                item => item.Id == p_id
            };
            LineItem updateItem = _lineItemDb.FindSingle(new(_configuration){
                Conditions = conditions
            });
            if (updateItem == null) throw new ArgumentException("Item with id not found.");

            updateItem.Quantity += p_quantity;
            _lineItemDb.Save();
        }

        public IList<Category> GetCategories() {
            return _categoryDb.Query(new(null)).OrderBy(c => c.Name).ToList();
        }
    }
}