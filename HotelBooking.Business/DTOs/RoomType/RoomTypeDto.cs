namespace HotelBooking.Business.DTOs.RoomType
{
    public class RoomTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public decimal RatePerNight { get; set; }
    }
}
