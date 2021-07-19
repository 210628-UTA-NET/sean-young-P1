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
            AddressManager addressManager,
            StateManager stateManager) {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _addressManager = addressManager;
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

        public async Task<IActionResult> OnPostChangeEmailAsync() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid) {
                await LoadAsync(user);
                return Page();
            }

            var email = await _userManager.GetEmailAsync(user);
            /*
            if (Input.NewEmail != email) {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new { userId = userId, email = Input.NewEmail, code = code },
                    protocol: Request.Scheme);
                await _emailSender.SendEmailAsync(
                    Input.NewEmail,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                StatusMessage = "Confirmation link to change email sent. Please check your email.";
                return RedirectToPage();
            }*/

            StatusMessage = "Your email is unchanged.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid) {
                await LoadAsync(user);
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
        }
    }
}
