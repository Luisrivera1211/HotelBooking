using HotelBooking.Data.Base;

namespace HotelBooking.Data.Entities
{
    public class Guest : AuditEntity
    {
        public string FullName { get; set; }

        public string Document { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

    }
}
