namespace HotelBooking.Data.Entities
{
    public sealed class AuditLog
    {
        public int Id { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public DateTime OccurredAt { get; set; }  
        public string? Details { get; set; }
    }
}
