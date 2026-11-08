using HotelBooking.Data.Base;

namespace HotelBooking.Data.Entities
{
    public class Block : AuditEntity
    {
        public string Name { get; set; }
        public int HotelId { get; set; }
        public bool IsActive { get; set; } = true;
        public List<Floor> Floors { get; set; } = new();

    }
}
