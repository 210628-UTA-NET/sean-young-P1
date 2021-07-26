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
using System.Text;

namespace SAWebUI.Controllers {
    public class OrderController : Controller {
        private readonly ILogger<InventoryController> _logger;
        private readonly OrderManager _orderManager;
        private readonly StorefrontManager _storefrontManager;
        private readonly UserManager<CustomerUser> _userManager;

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

        private static IList<Order> SortOrders(IList<Order> orders, string orderBy) {
            return orderBy switch {
                "date_desc" => orders.OrderByDescending(o => o.DatePlaced).ToList(),
                "date_asc" => orders.OrderBy(o => o.DatePlaced).ToList(),
                "price_desc" => orders.OrderByDescending(o => o.TotalAmount).ToList(),
                "price_asc" => orders.OrderBy(o => o.TotalAmount).ToList(),
                _ => orders,
            };
        }


        [Authorize]
        public async Task<IActionResult> Index(string orderBy) {
            try {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                IList<Order> results = _orderManager.QueryById(user.Id);

                return View(new OrderViewModel {
                    Title = "Your Orders",
                    Orders = SortOrders(results, orderBy),
                    OrderBy = orderBy
                });
            } catch (Exception e) {
                TempData["error"] = e.Message;
                return Redirect("~/");
            }
        }

        [Authorize(Roles = "Manager")]
        public IActionResult Storefront(string orderBy) {
            try {
                if (Request.Cookies["storefrontID"] == null) {
                    TempData["error"] = "No storefront selected";
                    return RedirectToAction(nameof(Index));
                }
                int storefrontId = int.Parse(Request.Cookies["storefrontID"]);
                Storefront sf = _storefrontManager.Get(storefrontId);
                if (sf == null) throw new ArgumentException("Unable to load storefront information");
                IList<Order> results = _orderManager.QueryById(storefrontId);

                return View(nameof(Index), new OrderViewModel {
                    Title = string.Format("Orders History From {0}", sf.Name),
                    Orders = SortOrders(results, orderBy),
                    OrderBy = orderBy
                });
            } catch (Exception e) {
                TempData["error"] = e.Message;
                return Redirect("~/");
            }

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
