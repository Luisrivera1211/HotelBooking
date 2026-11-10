using System.Net.Mail;

namespace HotelBooking.Business.Helpers
{
    public static class InputValidator
    {
        // Valida el formato del correo. Data solo comprueba que no venga vacío ni demasiado largo.
        public static bool IsValidEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var trimmed = email.Trim();
            try
            {
                return new MailAddress(trimmed).Address == trimmed;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
