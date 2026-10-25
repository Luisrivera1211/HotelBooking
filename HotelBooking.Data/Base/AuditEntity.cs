namespace HotelBooking.Data.Base
{
    public abstract class AuditEntity
    {
        public int Id { get; set; }
        public int GuestId { get; set; }
        public int Number { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }



    }
}
