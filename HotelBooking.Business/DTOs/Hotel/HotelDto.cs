namespace HotelBooking.Business.DTOs.Hotels
{
    public class HotelDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
