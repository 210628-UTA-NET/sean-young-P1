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
            CookieOptions options = new();
            options.HttpOnly = true;
            options.Secure = true;
            options.Expires = DateTime.Now.AddDays(1);
            Storefront result = _storefrontManager.Get(id);

            if (result == null) return RedirectToAction(nameof(Index));
            try {
                Response.Cookies.Append("storefrontID", result.Id.ToString(), options);
                Response.Cookies.Append("storefrontAddress", result.Address.ToString(), options);
            } catch (NullReferenceException) {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Search(HomeModel model, int? page) {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            try {
                int currentPage = page ?? 1;
                IList<Storefront> results = _storefrontManager.QueryByAddress(model.SearchString, currentPage);

                TempData["Storefronts"] = results;
                return View(nameof(Index));
            } catch (Exception) {
                return View(nameof(Index));
            }
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
