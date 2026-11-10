namespace HotelBooking.Business.DTOs.Block
{
    public class BlockDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int HotelId { get; set; }
    }
}
