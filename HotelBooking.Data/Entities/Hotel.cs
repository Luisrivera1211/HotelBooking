using HotelBooking.Data.Base;

namespace HotelBooking.Data.Entities
{
    public sealed class Hotel : AuditEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Stars { get; set; } = 3;
        public bool IsActive { get; set; } = true;
        public List<Block> Blocks { get; set; } = new();
    }
}
