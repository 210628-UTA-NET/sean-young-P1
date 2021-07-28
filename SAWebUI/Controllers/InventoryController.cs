using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SAWebUI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using SAModels;
using SABL;

namespace SAWebUI.Controllers {
    /// <summary>
    /// MVC Controller that queries a given storefront's inventory.
    /// </summary>
    public class InventoryController : Controller {
        private readonly ILogger<InventoryController> _logger;
        private readonly LineItemManager _lineItemManager;
        private readonly UserManager<CustomerUser> _userManager;


        /// <param name="logger">Logger interface</param>
        /// <param name="p_lineItemManager">BL module that handles Lineitems</param>
        /// <param name="p_userManager">ASP identity module that manages the users</param>
        public InventoryController(
            ILogger<InventoryController> logger, 
            LineItemManager p_lineItemManager,
            UserManager<CustomerUser> p_userManager) {
            _logger = logger;
            _lineItemManager = p_lineItemManager;
            _userManager = p_userManager;
        }

        /// <summary>
        /// The index controller. Contains a search bar and a list of 
        /// categories of products from the selected storefront.
        /// </summary>
        /// <returns>A view from which the user can search products</returns>
        public IActionResult Index() {
            try {
                if (Request.Cookies["storefrontID"] != null) {
                    int storefrontId = int.Parse(Request.Cookies["storefrontID"]);
                    IList<Category> results = _lineItemManager.GetCategories(storefrontId);
                    return View(results);
                } else {
                    return View();
                }
            } catch (Exception e) {
                TempData["error"] = e.Message;
                _logger.LogError("[INVENTORY] Error loading Index: {0}", e.ToString());
                return Redirect("~/");
            }
        }

        /// <summary>
        /// Searches the selected storefront for products with a name containing
        /// the related query and/or with the given category.
        /// </summary>
        /// <param name="query">The product name to search for</param>
        /// <param name="category">The name of the category to search</param>
        /// <returns>A view displaying the results of the search</returns>
        public IActionResult Search(string query, string category) {
            try {
                if (Request.Cookies["storefrontID"] == null) {
                    TempData["error"] = "No storefront selected";
                    return RedirectToAction(nameof(Index));
                }
                int storefrontId = int.Parse(Request.Cookies["storefrontID"]);
                IList<LineItem> results = _lineItemManager.QueryStoreInventory(storefrontId, query, category);
                _logger.LogInformation("[INVENTORY:SEARCH] Search: \"{0}\" returned: {1} items", query, results.Count);
                return View(new InventoryViewModel { Inventory = results });
            } catch (Exception e) {
                TempData["error"] = e.Message;
                _logger.LogError("[INVENTORY:SEARCH] Error with Inventory Search: \"{0}\"\n{1}", query ?? "NULL", e.ToString());
                string returnUrl = Request.Headers["Referer"];
                if (returnUrl != null) {
                    return Redirect(returnUrl);
                } else {
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        /// <summary>
        /// Controller which allows a manager to add a specified quantity to 
        /// the given item with the specified Id.
        /// </summary>
        /// <param name="itemId">The id of the item to replenish</param>
        /// <param name="quantity">The quantity to add to the given item</param>
        /// <returns></returns>
        [Authorize(Roles = "Manager")]
        public IActionResult Replenish(int itemId, int quantity) {
            try {
                _lineItemManager.ReplenishItem(itemId, quantity);
                TempData["success"] = string.Format("Successfully added {0} quantity to item", quantity);
                _logger.LogInformation("[INVENTORY:REPLENISH] Manager added {0} to item with ID {1}", itemId, quantity);
            } catch (ArgumentException e) {
                TempData["error"] = e.Message;
                _logger.LogError("[INVENTORY:REPLENISH] Error adding {0} to item with ID {1} \n{2}", itemId, quantity, e.ToString());
            }

            string returnUrl = Request.Headers["Referer"];
            if (returnUrl != null) {
                return Redirect(returnUrl);
            } else {
                return RedirectToAction(nameof(Index));
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            _logger.LogCritical("[INVENTORY] Uncaught error in Inventory Controller\n{0}", Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
