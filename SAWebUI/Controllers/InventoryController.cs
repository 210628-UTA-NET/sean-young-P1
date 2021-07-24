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
using System.Web;

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
            IList<Category> results = _lineItemManager.GetCategories();
            return View(results);
        }

        public IActionResult Search(string query, string category) {
            if (Request.Cookies["storefrontID"] == null) return RedirectToAction(nameof(Index));
            int storefrontId = int.Parse(Request.Cookies["storefrontID"]);
            IList<LineItem> results = _lineItemManager.QueryStoreInventory(storefrontId, query, category);

            //string errorMsg = (string) TempData["error"];
            return View(new InventoryViewModel{ Inventory = results });
        }

        [Authorize(Roles = "Manager")]
        public IActionResult Replenish(int itemId, int quantity) {
            try {
                _lineItemManager.ReplenishItem(itemId, quantity);
            } catch (ArgumentException e) {
                TempData["error"] = e.Message;
            }

            string returnUrl = Request.Headers["Referer"];
            if (returnUrl != null) {
                return Redirect(returnUrl);
            } else {
                return RedirectToAction(nameof(Search));
            }
        }

        [Authorize]
        public async Task<IActionResult> Order(InventoryViewModel model) {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            return View(nameof(Search));
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
