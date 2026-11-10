namespace HotelBooking.Business.Interfaces
{
    public interface IEmailSender
    {
        Task<bool> SendAsync(string to, string subject, string body);
    }
}
