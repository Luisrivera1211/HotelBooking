using System.ComponentModel.DataAnnotations;
using HotelBooking.Business.DTOs.Floor;

namespace HotelBooking.Web.Models
{
    
    public class FloorFormViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "El número de piso debe ser mayor que cero.")]
        public int Number { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un bloque.")]
        public int BlockId { get; set; }

        public CreateFloorDto ToDto() => new()
        {
                Number = Number,
                BlockId = BlockId,
        };
    }
}
