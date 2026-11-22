using System.ComponentModel.DataAnnotations;
using HotelBooking.Business.DTOs.Room;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelBooking.Web.Models
{
    public class RoomFormViewModel
    {
        public int? Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un piso.")]
        public int FloorId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un tipo de habitación.")]
        public int RoomTypeId { get; set; }

        [Required(ErrorMessage = "El número de habitación es obligatorio.")]
        [StringLength(50, ErrorMessage = "El número no puede superar 50 caracteres.")]
        public string Number { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(300, ErrorMessage = "La descripción no puede superar 300 caracteres.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Indique al menos una característica.")]
        [StringLength(300, ErrorMessage = "Las características no pueden superar 300 caracteres.")]
        public string Features { get; set; } = string.Empty;

        public List<SelectListItem> RoomTypes { get; set; } = new();

        public CreateRoomDto ToDto() => new()
        {
            FloorId = FloorId,
            RoomTypeId = RoomTypeId,
            Number = Number,
            Description = Description,
            Features = Features,
        };
    }
}
