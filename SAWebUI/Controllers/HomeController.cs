using Microsoft.AspNetCore.Http;
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
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly StorefrontManager _storefrontManager;

        public HomeController(ILogger<HomeController> logger, StorefrontManager p_storefrontManager) {
            _logger = logger;
            _storefrontManager = p_storefrontManager;
        }


        public IActionResult Index(HomeModel model) {
            return View(model);
        }


        public IActionResult Select(int id) {
            try {
                CookieOptions options = new();
                options.HttpOnly = true;
                options.Secure = true;
                options.Expires = DateTime.Now.AddDays(1);
                Storefront result = _storefrontManager.Get(id);
                if (result == null) return RedirectToAction(nameof(Index));
                Response.Cookies.Append("storefrontID", result.Id.ToString(), options);
                Response.Cookies.Append("storefrontAddress", result.Address.ToString(), options);
                _logger.LogInformation("[HOME:SELECT] User selected storefront ID: {0}, {1}", result.Id, result.Name);
            } catch (Exception e) {
                TempData["error"] = "Invalid Storefront ID";
                _logger.LogError("[HOME:SELECT] Error with Storefront select: id: {0} \n Exception: {1}", id, e.ToString());
            }
            return RedirectToAction(nameof(Index));

        }

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
