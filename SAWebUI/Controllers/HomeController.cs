using Microsoft.AspNetCore.Http;
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
    /// MVC Contoller that controls actions related to the Homepage
    /// </summary>
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly StorefrontManager _storefrontManager;

        /// <param name="logger">Logger interface</param>
        /// <param name="p_storefrontManager">BL module that queries storefronts</param>
        public HomeController(ILogger<HomeController> logger, StorefrontManager p_storefrontManager) {
            _logger = logger;
            _storefrontManager = p_storefrontManager;
        }

        /// <summary>
        /// The index controller. Loads the homepage from which the user can
        /// search and select a storefront.
        /// </summary>
        /// <param name="model">The input model for the home page</param>
        /// <returns>The homepage view</returns>
        public IActionResult Index(HomeModel model) {
            return View(model);
        }

        /// <summary>
        /// Controller which attaches a cookie to the user's browser such that
        /// the user can select a storefront from which they can query items,
        /// place items, and view orders.
        /// </summary>
        /// <param name="id">The Id of the storefront to select</param>
        /// <returns>Redirects the user back to the index</returns>
        public IActionResult Select(int id) {
            try {
                CookieOptions options = new();
                options.HttpOnly = true;
                options.Secure = true;
                options.Expires = DateTime.Now.AddDays(1);
                Storefront result = _storefrontManager.Get(id);
                if (result == null) throw new ArgumentException("No storefront exists with that id.");
                Response.Cookies.Append("storefrontID", result.Id.ToString(), options);
                Response.Cookies.Append("storefrontAddress", result.Address.ToString(), options);
                _logger.LogInformation("[HOME:SELECT] User selected storefront ID: {0}, {1}", result.Id, result.Name);
                TempData["success"] = "Storefront selected";
            } catch (Exception e) {
                TempData["error"] = "Invalid Storefront ID";
                _logger.LogError("[HOME:SELECT] Error with Storefront select: id: {0} \n Exception: {1}", id, e.ToString());
            }
            return RedirectToAction(nameof(Index));

        }

        /// <summary>
        /// Searches for storefronts based on input from the Homepage input model.
        /// </summary>
        /// <param name="model">The input model for the home page</param>
        /// <returns>
        /// Returns the index view page with a list containing the results of the query
        /// </returns>
        [HttpPost]
        public IActionResult Search(HomeModel model) {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            try {
                IList<Storefront> results = _storefrontManager.QueryByAddress(model.SearchString);
                TempData["Storefronts"] = results;
                _logger.LogInformation("[HOME:SEARCH] Search: \"{0}\" returned: {1} items", model.SearchString, results.Count);
            } catch (Exception e) {
                _logger.LogError("[HOME:SEARCH] Error with Storefront Search: \"{0}\"\n{1}", model.SearchString ?? "NULL", e.ToString());
                TempData["error"] = e.Message;
            }
            return View(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            _logger.LogCritical("[HOME] Uncaught error in Home Controller\n{0}", Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
