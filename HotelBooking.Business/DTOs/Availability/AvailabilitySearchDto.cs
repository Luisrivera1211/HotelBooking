namespace HotelBooking.Business.DTOs.Availability
{
    public class AvailabilitySearchDto
    {
        public int HotelId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Capacity { get; set; }
    }
}
