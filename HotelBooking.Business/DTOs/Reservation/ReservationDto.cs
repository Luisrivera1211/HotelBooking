namespace HotelBooking.Business.DTOs.Reservation
{
    public class ReservationDto
    {   
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int GuestId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Capacity { get; set; }
    }
}
