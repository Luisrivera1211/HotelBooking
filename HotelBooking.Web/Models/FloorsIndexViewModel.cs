namespace HotelBooking.Web.Models
{
    public class FloorsIndexViewModel
    {
        public HotelBooking.Business.DTOs.Block.BlockDto Block { get; set; } = null!;
        public List<HotelBooking.Business.DTOs.Floor.FloorDto> Floors { get; set; } = new();
    }
}
