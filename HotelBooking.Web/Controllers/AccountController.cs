using HotelBooking.Business.DTOs.Employee;
using HotelBooking.Business.Interfaces;
using HotelBooking.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Policy;

namespace HotelBooking.Web.Controllers
{
    [AllowAnonymous]
    public class AccountController : AppController
    {
        private readonly IEmployeeService _employeeService;

        public AccountController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel dto, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(dto);

            var result = await _employeeService.LoginAsync(dto.ToDto());
            if (!result.Success)
            {
                AddError(result);
                dto.Password = string.Empty;
                return View(dto);
            }

            var employee = result.Data!;
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, employee.Id.ToString()),
                new(ClaimTypes.Name, employee.FullName),
                new(ClaimTypes.Email, employee.Email),
                new(ClaimTypes.Role, employee.Role.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
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
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
