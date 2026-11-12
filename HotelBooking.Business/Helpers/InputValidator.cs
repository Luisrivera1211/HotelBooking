using System.Net.Mail;

namespace HotelBooking.Business.Helpers
{
    public static class InputValidator
    {
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
