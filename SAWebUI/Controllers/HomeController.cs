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


        public IActionResult Index() {
            return View();
        }


        public IActionResult Select(int id) {
            CookieOptions options = new();
            options.Expires = DateTime.Now.AddDays(1);
            //Response.Cookies.Append("storefrontAddress", id.ToString(), options);
            Response.Cookies.Append("storefrontAddress", "Testing", options);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Search(HomeViewModel model, int? page) {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));

            int currentPage = page ?? 1;
            IList<Storefront> results = _storefrontManager.QueryByAddress(model.SearchString, currentPage);
            return RedirectToAction(nameof(Index));
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
