using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using SAModels;
using System.Linq;
using System.Threading.Tasks;

namespace SAWebUI.Controllers {

    //[Authorize(Roles = "Manager")]
    public class RoleController : Controller {
        RoleManager<CustomerUser> roleManager;
        UserManager<CustomerUser> userManager;

        public RoleController(RoleManager<CustomerUser> roleManager, UserManager<CustomerUser> userManager) {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        public IActionResult Index() {
            var roles = roleManager.Roles.ToList();
            return View(roles);
        }

        public IActionResult Create() {
            return View(new CustomerUser());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CustomerUser role) {
            await roleManager.CreateAsync(role);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Register() {
            //var user = userManager.FindById("de5a23c3-27d9-42c7-9b56-73c981672231");
            var user = await userManager.FindByIdAsync("de5a23c3-27d9-42c7-9b56-73c981672231");
            await userManager.AddToRoleAsync(user, "Manager");
            return RedirectToAction("Index");
        }
    }
}
