namespace HotelBooking.Web.Models
{
    public class BlocksIndexViewModel
    {
        public HotelBooking.Business.DTOs.Hotels.HotelDto Hotel { get; set; } = null!;
        public List<HotelBooking.Business.DTOs.Block.BlockDto> Blocks { get; set; } = new();
    }
}
