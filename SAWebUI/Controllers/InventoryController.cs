using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SAWebUI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using SAModels;
using SABL;

namespace SAWebUI.Controllers {
    public class InventoryController : Controller {
        private readonly ILogger<InventoryController> _logger;
        private readonly LineItemManager _lineItemManager;
        private readonly UserManager<CustomerUser> _userManager;

        public InventoryController(
            ILogger<InventoryController> logger, 
            LineItemManager p_lineItemManager,
            UserManager<CustomerUser> p_userManager) {
            _logger = logger;
            _lineItemManager = p_lineItemManager;
            _userManager = p_userManager;
        }


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
                return RedirectToAction(nameof(Search));
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            _logger.LogCritical("[INVENTORY] Uncaught error in Inventory Controller\n{0}", Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
