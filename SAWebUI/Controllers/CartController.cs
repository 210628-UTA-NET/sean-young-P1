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
    public class CartController : Controller {
        private readonly ILogger<InventoryController> _logger;
        private readonly ShoppingCartManager _shoppingCartManager;
        private readonly UserManager<CustomerUser> _userManager;

        public CartController(
            ILogger<InventoryController> logger, 
            ShoppingCartManager p_shoppingCartManager,
            UserManager<CustomerUser> p_userManager) {
            _logger = logger;
            _shoppingCartManager = p_shoppingCartManager;
            _userManager = p_userManager;
        }


        [Authorize]
        public async Task<IActionResult> Index() {
            try {
                if (Request.Cookies["storefrontID"] == null) {
                    TempData["error"] = "No storefront selected";
                    return Redirect("~/");
                }
                int storefrontId = int.Parse(Request.Cookies["storefrontID"]);
                var user = await _userManager.GetUserAsync(User);
                if (user == null) {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                ShoppingCart userCart = _shoppingCartManager.GetCart(user.Id, storefrontId);
                return View(userCart);
            } catch (Exception e) {
                TempData["error"] = e.Message;
                return Redirect("~/");
            }
        }

        [Authorize]
        public async Task<IActionResult> AddItem(int itemId, int quantity) {
            try {
                if (Request.Cookies["storefrontID"] == null) {
                    TempData["error"] = "No storefront selected";
                    return Redirect("~/");
                }
                int storefrontId = int.Parse(Request.Cookies["storefrontID"]);
                var user = await _userManager.GetUserAsync(User);
                if (user == null) {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                _shoppingCartManager.AddItem(itemId, storefrontId, user.Id, quantity);
            } catch (Exception e) {
                TempData["error"] = e.Message;
            }

            string returnUrl = Request.Headers["Referer"];
            if (returnUrl != null) {
                return Redirect(returnUrl);
            } else {
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize]
        public async Task<IActionResult> RemoveItem(int itemId) {
            try {
                if (Request.Cookies["storefrontID"] == null) {
                    TempData["error"] = "No storefront selected";
                    return Redirect("~/");
                }
                int storefrontId = int.Parse(Request.Cookies["storefrontID"]);
                var user = await _userManager.GetUserAsync(User);
                if (user == null) {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                _shoppingCartManager.RemoveItem(itemId, user.Id, storefrontId);
            } catch (Exception e) {
                TempData["error"] = e.Message;
            }
            string returnUrl = Request.Headers["Referer"];
            if (returnUrl != null) {
                return Redirect(returnUrl);
            } else {
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize]
        public async Task<IActionResult> RemoveAll() {
            try {
                if (Request.Cookies["storefrontID"] == null) {
                    TempData["error"] = "No storefront selected";
                    return Redirect("~/");
                }
                int storefrontId = int.Parse(Request.Cookies["storefrontID"]);
                var user = await _userManager.GetUserAsync(User);
                if (user == null) {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                _shoppingCartManager.RemoveAll(user.Id, storefrontId);
            } catch (Exception e) {
                TempData["error"] = e.Message;
            }
            string returnUrl = Request.Headers["Referer"];
            if (returnUrl != null) {
                return Redirect(returnUrl);
            } else {
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize]
        public async Task<IActionResult> Order() {
            try {
                if (Request.Cookies["storefrontID"] == null) {
                    TempData["error"] = "No storefront selected";
                    return Redirect("~/");
                }
                int storefrontId = int.Parse(Request.Cookies["storefrontID"]);
                var user = await _userManager.GetUserAsync(User);
                if (user == null) {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                _shoppingCartManager.PlaceOrder(user.Id, storefrontId);
            } catch (Exception e) {
                TempData["error"] = e.Message;
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
