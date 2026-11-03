namespace HotelBooking.Business.DTOs.Room
{
    public class CreateRoomDto
    {
        public int FloorId { get; set; }

        public int RoomTypeId { get; set; }

        public string Number { get; set; }
    }
}
