using Microsoft.AspNetCore.Authorization;
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
    public class InventoryController : Controller {
        private readonly ILogger<InventoryController> _logger;
        private readonly LineItemManager _lineItemManager;

        public InventoryController(ILogger<InventoryController> logger, LineItemManager p_lineItemManager) {
            _logger = logger;
            _lineItemManager = p_lineItemManager;
        }


        public IActionResult Index() {
            return View();
        }

        public IActionResult Search(string query, string orderby, int? page) {
            int storefrontId = int.Parse(Request.Cookies["storefrontID"]);
            _lineItemManager.QueryInventory(storefrontId, query, null, 1);
            return View();
        }

        [Authorize(Roles = "Manager")]
        public IActionResult Replenish(int id, int quantity) {
            return View(nameof(Index));
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
