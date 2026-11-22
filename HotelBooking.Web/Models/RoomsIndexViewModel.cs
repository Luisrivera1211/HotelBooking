namespace HotelBooking.Web.Models
{
    public class RoomsIndexViewModel
    {
        public HotelBooking.Business.DTOs.Floor.FloorDto Floor { get; set; } = null!;
        public List<HotelBooking.Business.DTOs.Room.RoomDto> Rooms { get; set; } = new();
    }
}
