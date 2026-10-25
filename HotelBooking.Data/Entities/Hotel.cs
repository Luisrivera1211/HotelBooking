using HotelBooking.Data.Base;

namespace HotelBooking.Data.Entities
{
    public sealed class Hotel : AuditEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }

    }
}
