﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SAWebUI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SAWebUI.Controllers {
    public class CustomerController : Controller {
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ILogger<CustomerController> logger) {
            _logger = logger;
        }

        [Authorize(Roles = "Manager")]
        public IActionResult Index() {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
