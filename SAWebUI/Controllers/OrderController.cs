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
    /// <summary>
    /// MVC Controller that allows the user to query orders.
    /// </summary>
    public class OrderController : Controller {
        private readonly ILogger<InventoryController> _logger;
        private readonly OrderManager _orderManager;
        private readonly StorefrontManager _storefrontManager;
        private readonly UserManager<CustomerUser> _userManager;

        /// <param name="logger">Logger interface</param>
        /// <param name="p_orderManager">BL module that handles the order</param>
        /// <param name="p_userManager">ASP identity module that manages the users</param>
        /// <param name="p_storefrontManager">BL module that handles the storefronts</param>
        public OrderController(
            ILogger<InventoryController> logger, 
            OrderManager p_orderManager,
            UserManager<CustomerUser> p_userManager,
            StorefrontManager p_storefrontManager) {
            _logger = logger;
            _orderManager = p_orderManager;
            _userManager = p_userManager;
            _storefrontManager = p_storefrontManager;
        }

        /// <summary>
        /// Helper function which sorts the order results by the given parameters
        /// </summary>
        /// <param name="orders">The list of orders to sort</param>
        /// <param name="orderBy">The order to sort the list of orders</param>
        /// <returns></returns>
        private static IList<Order> SortOrders(IList<Order> orders, string orderBy) {
            return orderBy switch {
                "date_desc" => orders.OrderByDescending(o => o.DatePlaced).ToList(),
                "date_asc" => orders.OrderBy(o => o.DatePlaced).ToList(),
                "price_desc" => orders.OrderByDescending(o => o.TotalAmount).ToList(),
                "price_asc" => orders.OrderBy(o => o.TotalAmount).ToList(),
                _ => orders,
            };
        }

        /// <summary>
        /// The index controller. Loads the user's orders.
        /// </summary>
        /// <param name="orderBy">The order by which to sort the results</param>
        /// <returns>A view containing with the list of user's orders</returns>
        [Authorize]
        public async Task<IActionResult> Index(string orderBy) {
            try {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                IList<Order> results = _orderManager.QueryById(user.Id);
                _logger.LogInformation("[ORDER:INDEX] Search for order history of Customer ID: {0} returned {1} items", user.Id, results.Count);
                return View(new OrderViewModel {
                    Title = "Your Orders",
                    Orders = SortOrders(results, orderBy),
                    OrderBy = orderBy
                });
            } catch (Exception e) {
                TempData["error"] = e.Message;
                _logger.LogError("[ORDER:INDEX] Error searching order history of customer", e.ToString());
                return Redirect("~/");
            }
        }

        /// <summary>
        /// Loads all orders from the given storefront.
        /// </summary>
        /// <param name="orderBy">The order by which to sort the results</param>
        /// <returns>A view containing with the list of storefront's orders</returns>
        [Authorize(Roles = "Manager")]
        public IActionResult Storefront(string orderBy) {
            try {
                if (Request.Cookies["storefrontID"] == null) {
                    TempData["error"] = "No storefront selected";
                    return Redirect("~/");
                }
                int storefrontId = int.Parse(Request.Cookies["storefrontID"]);
                Storefront sf = _storefrontManager.Get(storefrontId);
                if (sf == null) throw new ArgumentException("Unable to load storefront information");
                IList<Order> results = _orderManager.QueryById(storefrontId);
                _logger.LogInformation("[ORDER:STOREFRONT] Search for storefront ID: {0} returned {1} items", storefrontId, results.Count);
                return View(nameof(Index), new OrderViewModel {
                    Title = string.Format("Orders History From {0}", sf.Name),
                    Orders = SortOrders(results, orderBy),
                    OrderBy = orderBy
                });
            } catch (Exception e) {
                TempData["error"] = e.Message;
                _logger.LogError("[ORDER:STOREFRONT] Error searching order history of storefront: {0}", e.ToString());
                return Redirect("~/");
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            _logger.LogCritical("[ORDER] Uncaught error in Order Controller\n{0}", Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
