using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SAWebUI.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SAModels;
using SABL;

namespace SAWebUI.Controllers {
    /// <summary>
    /// MVC Controller that controls the actions related to the user's cart.
    /// </summary>
    public class CartController : Controller {
        private readonly ILogger<InventoryController> _logger;
        private readonly ShoppingCartManager _shoppingCartManager;
        private readonly UserManager<CustomerUser> _userManager;

        /// <param name="logger">Logger interface</param>
        /// <param name="p_shoppingCartManager">BL module that handles the Cart</param>
        /// <param name="p_userManager">ASP identity module that manages the users</param>
        public CartController(
            ILogger<InventoryController> logger, 
            ShoppingCartManager p_shoppingCartManager,
            UserManager<CustomerUser> p_userManager) {
            _logger = logger;
            _shoppingCartManager = p_shoppingCartManager;
            _userManager = p_userManager;
        }

        /// <summary>
        /// The index controller. Loads the user and storefront and then 
        /// attempts to load the user's cart at that storefront and display it.
        /// </summary>
        /// <returns>
        /// A view containing the user's cart contents. Will redirect to home
        /// in the event of an error.
        /// </returns>
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
                    _logger.LogError("[CART:INDEX] Unable to load user with ID: {0}", user.Id);
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                ShoppingCart userCart = _shoppingCartManager.GetCart(user.Id, storefrontId);
                return View(userCart);
            } catch (Exception e) {
                TempData["error"] = e.Message;
                _logger.LogError("[CART:INDEX] Failed to load cart\n {0}", e.ToString());
                return Redirect("~/");
            }
        }

        /// <summary>
        /// Adds the item with the given Id and the given quantity to the user's
        /// cart.
        /// </summary>
        /// <param name="itemId">The Id of the item to add</param>
        /// <param name="quantity">The quantity of the item to add</param>
        /// <returns>Redirects back to original URL or home in case of error</returns>
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
                    _logger.LogError("[CART:ADDITEM] Unable to load user with ID: {0}", user.Id);
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                _shoppingCartManager.AddItem(itemId, storefrontId, user.Id, quantity);
                TempData["success"] = "Successfully added to your cart";
                _logger.LogInformation("[CART:ADDITEM] User: {0} removed item with ID: {1} from cart at storefront ID: {2}", user.Id, itemId, storefrontId);
            } catch (Exception e) {
                TempData["error"] = e.Message;
                _logger.LogError("[CART:ADDITEM] Error adding item with ID {0} with quantity {1} to cart: {2}", itemId, quantity, e.ToString());
            }

            string returnUrl = Request.Headers["Referer"];
            if (returnUrl != null) {
                return Redirect(returnUrl);
            } else {
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Removes the item with the given Id from the user's cart.
        /// </summary>
        /// <param name="itemId">The Id of the item to remove</param>
        /// <returns>Redirects back to original URL or home in case of error</returns>
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
                    _logger.LogError("[CART:REMOVEITEM] Unable to load user with ID: {0}", user.Id);
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                _shoppingCartManager.RemoveItem(itemId, user.Id, storefrontId);
                TempData["success"] = "Successfully removed item with from your cart";
                _logger.LogInformation("[CART:REMOVEITEM] User: {0} removed item with id: {1} from cart at storefront ID: {2}", user.Id, itemId, storefrontId);
            } catch (Exception e) {
                TempData["error"] = e.Message;
                _logger.LogError("[CART:REMOVEITEM] Unable to remove item with ID: {0}\n{1}", itemId, e.ToString());
            }
            string returnUrl = Request.Headers["Referer"];
            if (returnUrl != null) {
                return Redirect(returnUrl);
            } else {
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Removes all items from the user's cart.
        /// </summary>
        /// <returns>Redirects back to original URL or home in case of error</returns>
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
                    _logger.LogError("[CART:REMOVEALL] Unable to load user with ID: {0}", user.Id);
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                _shoppingCartManager.RemoveAll(user.Id, storefrontId);
                TempData["success"] = "Your cart is now empty";
                _logger.LogInformation("[CART:REMOVEALL] User: {0} removed all items from cart at storefront ID: {1}", user.Id, storefrontId);
            } catch (Exception e) {
                TempData["error"] = e.Message;
                _logger.LogError("[CART:REMOVEALL] Failed to remove all items from cart: {0}\n", e.ToString());
            }
            string returnUrl = Request.Headers["Referer"];
            if (returnUrl != null) {
                return Redirect(returnUrl);
            } else {
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Converts the contents of the user's cart into an order.
        /// </summary>
        /// <returns>Redirects back to original URL or home in case of error</returns>
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
                    _logger.LogError("[CART:ORDER] Unable to load user with ID: {0}", user.Id);
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                _shoppingCartManager.PlaceOrder(user.Id, storefrontId);
                TempData["success"] = "Order placed";
                _logger.LogInformation("[CART:ORDER] User: {0} placed order at storefront ID: {1}", user.Id, storefrontId);
            } catch (Exception e) {
                TempData["error"] = e.Message;
                _logger.LogError("[CART:ORDER] Error placing order: {0}", e.ToString());
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
            _logger.LogCritical("[CART] Uncaught error in Cart Controller\n{0}", Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
