using HotelBooking.Data.Base;

namespace HotelBooking.Data.Entities
{
    public sealed class Floor : AuditEntity
    {
        public int Number { get; set; }

        public bool IsActive { get; set; } = true; 

        public int BlockId { get; set; }

        public Block Block { get; set; } = null!;

        public List<Room> Rooms { get; set; } = new();
    }
}
