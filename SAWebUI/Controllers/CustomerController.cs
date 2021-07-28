using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SAWebUI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SABL;

namespace SAWebUI.Controllers {
    /// <summary>
    /// MVC Controller that queries the database of customers.
    /// </summary>
    public class CustomerController : Controller {
        private readonly ILogger<CustomerController> _logger;
        private readonly CustomerManager _customerManager;

        /// <param name="logger">Logger interface</param>
        /// <param name="p_customerManager">BL module that manages customer queries</param>
        public CustomerController(ILogger<CustomerController> logger, CustomerManager p_customerManager) {
            _logger = logger;
            _customerManager = p_customerManager;
        }

        /// <summary>
        /// Index controller. Loads a view from which a manager can query 
        /// customers by name.
        /// </summary>
        /// <param name="query">The search parameter</param>
        /// <param name="page">The page of results to return (if applicable)</param>
        /// <returns>
        /// A view containing the search bar and the results of any customer
        /// searches.
        /// </returns>
        [Authorize(Roles = "Manager")]
        public IActionResult Index(string query, int? page) {
            try {
                if (query == null) return View();
                page = (page == null) ? 1 : page;
                IList<CustomerViewModel> results = _customerManager.QueryByName(query, (int)page).Select(c => new CustomerViewModel(c)).ToList();
                _logger.LogInformation("[CUSTOMER:INDEX] Customer search: \"{0}\" returned {1} results", query, results.Count);
                return View(results);
            } catch (Exception e) {
                TempData["error"] = e.Message;
                _logger.LogError("[CUSTOMER:INDEX] Error searching customers: \"{0}\" \n{1}", query, e.ToString());
                string returnUrl = Request.Headers["Referer"];
                if (returnUrl != null) {
                    return Redirect(returnUrl);
                } else {
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            _logger.LogCritical("[CUSTOMER] Uncaught error in Customer Controller\n{0}", Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
