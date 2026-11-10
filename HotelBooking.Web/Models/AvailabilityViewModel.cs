using HotelBooking.Business.DTOs.Hotels;
using HotelBooking.Business.DTOs.Room;

namespace HotelBooking.Web.Models
{
    public class AvailabilityViewModel
    {
        public AvailabilitySearchViewModel Search { get; set; } = new();
        public List<HotelDto> Hotels { get; set; } = new();
        // null = aún no se ha buscado; lista vacía = buscó y no hay nada
        public List<RoomDto>? Rooms { get; set; }
    }
}
