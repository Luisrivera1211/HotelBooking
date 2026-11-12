using HotelBooking.Data.Results;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers
{
    public abstract class AppController : Controller
    {
        protected void FlashOk(string message) => TempData["Ok"] = message;
        protected void FlashError(string message) => TempData["Error"] = message;   // para el PRÓXIMO request (redirect)

        protected void ShowError(string message) => ViewData["Error"] = message;

        protected IActionResult RedirectError(string message, string action, string? controller = null, object? routeValues = null)
        {
            FlashError(message);
            return RedirectToAction(action, controller, routeValues);
        }

        protected void AddError<T>(OperationResult<T> result) => ModelState.AddModelError(string.Empty, result.Message);

        protected IActionResult FlashAndRedirect<T>(OperationResult<T> result, string okMessage, string action,
            string? controller = null, object? routeValues = null)
        {
            if (!result.Success)
                FlashError(result.Message);
            else
                FlashOk(string.IsNullOrWhiteSpace(okMessage) ? "Operación realizada." : okMessage);

            return RedirectToAction(action, controller, routeValues);
        }
    }
}
