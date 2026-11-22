using System.Security.Claims;
using HotelBooking.Business.Interfaces;
using HotelBooking.Web.Constants;
using HotelBooking.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers
{
    [AllowAnonymous]
    public class AccountController : AppController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IGuestService _guestService;

        public AccountController(IEmployeeService employeeService, IGuestService guestService)
        {
            _employeeService = employeeService;
            _guestService = guestService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var credentials = dto.ToDto();

            var employee = await _employeeService.LoginAsync(credentials);
            if (employee.Success)
            {
                var e = employee.Data!;
                await SignInAsync(e.Id, e.FullName, e.Email, e.Role.ToString());
                return RedirectAfterLogin(dto.ReturnUrl);
            }

            var guest = await _guestService.LoginAsync(credentials);
            if (guest.Success)
            {
                var g = guest.Data!;
                await SignInAsync(g.Id, g.FullName, g.Email, Roles.Guest);
                return RedirectAfterLogin(dto.ReturnUrl);
            }

            AddError(guest);
            dto.Password = string.Empty;
            return View(dto);
        }

        [HttpGet]
        public IActionResult RegisterGuest(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View(new RegisterGuestViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> RegisterGuest(RegisterGuestViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _guestService.RegisterAccountAsync(model.ToDto());
            if (!result.Success)
            {
                AddError(result);
                model.Password = model.ConfirmPassword = string.Empty;
                return View(model);
            }

            var g = result.Data!;
            await SignInAsync(g.Id, g.FullName, g.Email, Roles.Guest);
            FlashOk("Cuenta creada. Ya puede reservar.");
            return RedirectAfterLogin(model.ReturnUrl);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _employeeService.RegisterAsync(dto.ToDto());
            if (!result.Success)
            {
                AddError(result);
                dto.Password = dto.ConfirmPassword = string.Empty;
                return View(dto);
            }

            FlashOk("Cuenta creada. Ya puede iniciar sesión.");
            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private async Task SignInAsync(int id, string name, string email, string role)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, id.ToString()),
                new(ClaimTypes.Name, name),
                new(ClaimTypes.Email, email),
                new(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        }

        private IActionResult RedirectAfterLogin(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
