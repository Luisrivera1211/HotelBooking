namespace HotelBooking.Business.Services.Channels
{
    internal interface IEmailSender
    {
        Task<bool> SendAsync(string to, string subject, string htmlBody, string? plainBody = null);
    }
}
