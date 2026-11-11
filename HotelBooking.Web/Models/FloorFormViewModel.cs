using System.ComponentModel.DataAnnotations;
using HotelBooking.Business.DTOs.Floor;

namespace HotelBooking.Web.Models
{
    // Modelo del formulario: la validación de entrada vive en la capa de presentación;
    // Business recibe un DTO sin anotaciones y vuelve a validar por su cuenta.
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
