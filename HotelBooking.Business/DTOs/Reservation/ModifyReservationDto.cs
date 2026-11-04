namespace HotelBooking.Business.DTOs.Reservation
{
    public class ModifyReservationDto
    {   
        public int RoomId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Capacity { get; set; }
    }
}
