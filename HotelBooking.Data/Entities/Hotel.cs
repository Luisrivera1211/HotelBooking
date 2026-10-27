using HotelBooking.Data.Base;

namespace HotelBooking.Data.Entities
{
    public sealed class Hotel : AuditEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; }
        public bool IsActive { get; set; } = true;

        public List<Floor> Floors { get; set; } = new();
    }
}
