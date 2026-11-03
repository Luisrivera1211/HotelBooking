namespace HotelBooking.Business.DTOs.RoomType
{
    public class CreateRoomTypeDto
    {
        public string Name { get; set; }
        public int Capacity { get; set; }
        public decimal RatePerNight { get; set; }
    }
}
