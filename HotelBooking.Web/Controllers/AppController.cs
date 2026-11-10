using HotelBooking.Data.Results;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers
{
    // Base de todos los controladores: mensajes flash y manejo uniforme de OperationResult
    public abstract class AppController : Controller
    {
        protected void FlashOk(string message) => TempData["Ok"] = message;
        protected void FlashError(string message) => TempData["Error"] = message;   // para el PRÓXIMO request (redirect)

        // Para mostrar en ESTA respuesta (return View): TempData se filtraría a la siguiente petición
        protected void ShowError(string message) => ViewData["Error"] = message;

        // Fallo al CARGAR algo (no a guardar): mensaje y redirección
        protected IActionResult RedirectError(string message, string action, string? controller = null, object? routeValues = null)
        {
            FlashError(message);
            return RedirectToAction(action, controller, routeValues);
        }

        // Fallo al GUARDAR un formulario: el mensaje de negocio se muestra arriba del formulario
        protected void AddError<T>(OperationResult<T> result) => ModelState.AddModelError(string.Empty, result.Message);

        // Resultado de una acción simple (activar, confirmar, cancelar): flash + volver
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
