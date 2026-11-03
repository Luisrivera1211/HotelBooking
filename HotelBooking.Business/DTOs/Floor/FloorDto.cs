namespace HotelBooking.Business.DTOs.Floor
{
    public class FloorDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public bool IsActive { get; set; }
        public int HotelId { get; set; }
    }
}
