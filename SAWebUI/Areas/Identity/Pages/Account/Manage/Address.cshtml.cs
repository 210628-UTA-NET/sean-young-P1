using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using SABL;
using SAModels;
using Microsoft.EntityFrameworkCore;

namespace SAWebUI.Areas.Identity.Pages.Account.Manage {
    public partial class AddressModel : PageModel {
        private readonly UserManager<CustomerUser> _userManager;
        private readonly SignInManager<CustomerUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly AddressManager _addressManager;
        private readonly StateManager _stateManager;

        public AddressModel(
            UserManager<CustomerUser> userManager,
            SignInManager<CustomerUser> signInManager,
            IEmailSender emailSender,
            StateManager stateManager) {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _stateManager = stateManager;
        }

        public IEnumerable<State> States { get; set; }

        public string Username { get; set; }

        public Address Address { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel {

            [Display(Name = "Street Address")]
            public string StreetAddress { get; set; }

            [Display(Name = "City")]
            public string City { get; set; }

            [Display(Name = "State")]
            public string State { get; set; }
            //public string Country { get; set; }

            [Display(Name = "Zip Code")]
            public string ZipCode { get; set; }

        }

        private async Task LoadAsync(CustomerUser user) {

            var id = await _userManager.GetUserIdAsync(user);

            var addressUser = await _userManager.Users
                .Include(u => u.Address)
                .ThenInclude(a => a.State)
                .SingleAsync(u => u.Id == id);

            Address = addressUser.Address;
            States = _stateManager.GetAllStates();

            if (Address == null) {
                Address = new Address() {
                    State = States.First(s => s.Code == "00")
                };
            }

            Input = new InputModel() {
                StreetAddress = Address.StreetAddress,
                City = Address.City,
                State = Address.State.Code,
                ZipCode = Address.ZipCode,
            };
        }

        public async Task<IActionResult> OnGetAsync() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid) {
                await LoadAsync(user);
                return Page();
            }

            var addressUser = await _userManager.Users
                .Include(u => u.Address)
                .ThenInclude(a => a.State)
                .SingleAsync(u => u.Id == user.Id);

            Address = addressUser.Address;
            States = _stateManager.GetAllStates();

            if (Address == null) {
                Address = new Address() {
                    State = States.First(s => s.Code == "00")
                };
            }

            bool shouldUpdate = false;

            if (Input.StreetAddress != Address.StreetAddress) {
                Address.StreetAddress = Input.StreetAddress;
                shouldUpdate = true;
            }
            if (Input.City != Address.City) {
                Address.City = Input.City;
                shouldUpdate = true;
            }
            if (Input.State != Address.State.Code) {
                Address.State = States.First(s => s.Code == Input.State);
                shouldUpdate = true;
            }
            if (Input.ZipCode != Address.ZipCode) {
                Address.ZipCode = Input.ZipCode;
                shouldUpdate = true;
            }

            if (shouldUpdate) {
                /*
                if (Address.Id == 0) {
                    _addressManager.Insert(Address);
                } else {
                    _addressManager.Update(Address);
                }
                */
                user.Address = Address;
                await _userManager.UpdateAsync(user);
                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Your address has been updated";
            } else {
                StatusMessage = "No changes to your address were found";
            }
            return RedirectToPage();
        }
    }
}
