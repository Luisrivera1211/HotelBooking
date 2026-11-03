using HotelBooking.Data.Enums;

namespace HotelBooking.Business.DTOs.Reservation
{
    public class ReservationDto
    {   
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int GuestId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int GuestCount { get; set; }
        public decimal Total { get; set; }
        public ReservationStatus Status { get; set; }
        public int RoomNumber { get; set; }
        public string GuestName { get; set; } = string.Empty;
    }
}
