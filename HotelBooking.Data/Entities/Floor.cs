using HotelBooking.Data.Base;

namespace HotelBooking.Data.Entities
{
    public sealed class Floor : AuditEntity
    {
        public int HotelId { get; set; }
  
    }
}
