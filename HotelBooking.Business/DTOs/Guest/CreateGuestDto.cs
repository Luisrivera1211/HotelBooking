namespace HotelBooking.Business.DTOs.Guest
{
    public class CreateGuestDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
