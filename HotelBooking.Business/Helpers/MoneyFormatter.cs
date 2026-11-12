using System.Globalization;

namespace HotelBooking.Business.Helpers
{

    public static class MoneyFormatter
    {
        private static readonly NumberFormatInfo Dop = new()
        {
            CurrencySymbol = "RD$",
            CurrencyDecimalDigits = 2,
            CurrencyDecimalSeparator = ".",
            CurrencyGroupSeparator = ",",
            CurrencyPositivePattern = 2,  
            CurrencyNegativePattern = 9  
        };

        public static string ToDop(this decimal amount) => amount.ToString("C", Dop);
    }
}
