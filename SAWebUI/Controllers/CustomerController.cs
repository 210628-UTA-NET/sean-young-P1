﻿using Microsoft.AspNetCore.Authorization;
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
    public class CustomerController : Controller {
        private readonly ILogger<CustomerController> _logger;
        private readonly CustomerManager _customerManager;

        public CustomerController(ILogger<CustomerController> logger, CustomerManager p_customerManager) {
            _logger = logger;
            _customerManager = p_customerManager;
        }

        [Authorize(Roles = "Manager")]
        public IActionResult Index(string query, int? page) {
            try {
                if (query == null) return View();
                page = (page == null) ? 1 : page;
                IList<CustomerViewModel> results = _customerManager.QueryByName(query, (int)page).Select(c => new CustomerViewModel(c)).ToList();
                return View(results);
            } catch (Exception) {
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
