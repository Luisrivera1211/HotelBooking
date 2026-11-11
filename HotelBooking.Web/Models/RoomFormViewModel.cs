using System.ComponentModel.DataAnnotations;
using HotelBooking.Business.DTOs.Room;

namespace HotelBooking.Web.Models
{
    // Modelo del formulario: la validación de entrada vive en la capa de presentación;
    // Business recibe un DTO sin anotaciones y vuelve a validar por su cuenta.
    public class RoomFormViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un piso.")]
        public int FloorId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un tipo de habitación.")]
        public int RoomTypeId { get; set; }

        [Required(ErrorMessage = "El número de habitación es obligatorio.")]
        [StringLength(50, ErrorMessage = "El número no puede superar 50 caracteres.")]
        public string Number { get; set; } = string.Empty;

        public CreateRoomDto ToDto() => new()
        {
                FloorId = FloorId,
                RoomTypeId = RoomTypeId,
                Number = Number,
        };
    }
}
