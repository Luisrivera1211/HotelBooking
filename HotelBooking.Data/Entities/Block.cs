using HotelBooking.Data.Base;

namespace HotelBooking.Data.Entities
{
    public sealed class Block : AuditEntity
    {
        public string Name { get; set; } = string.Empty;
        public int HotelId { get; set; }
        public bool IsActive { get; set; } = true;
        public Hotel Hotel { get; set; } = null!;
        public List<Floor> Floors { get; set; } = new();

    }
}
