namespace HotelBooking.Web.Notifications
{
    public interface IEmailSender
    {
        Task<bool> SendAsync(string to, string subject, string htmlBody, string? plainBody = null);
    }
}
