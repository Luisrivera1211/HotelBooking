using HotelBooking.Data.Enums;

namespace HotelBooking.Business.DTOs.Room
{
    public class RoomDto
    {
        public int Id { get; set; }
        public int FloorId { get; set; }
        public string Number { get; set; }
        public RoomStatus Status { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public decimal RatePerNight { get; set; }
    }
}
