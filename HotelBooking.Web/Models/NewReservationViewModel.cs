using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Web.Models
{
    // Datos del formulario de nueva reserva. El huésped se identifica por documento (ya existe o se crea antes).
    public class NewReservationViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una habitación.")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "El documento del huésped es obligatorio.")]
        [StringLength(50, ErrorMessage = "El documento no puede superar 50 caracteres.")]
        public string Document { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime CheckIn { get; set; }

        [DataType(DataType.Date)]
        public DateTime CheckOut { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad de huéspedes debe ser mayor que cero.")]
        public int GuestCount { get; set; } = 1;

        // Solo para mostrar
        public string RoomLabel { get; set; } = string.Empty;
    }
}
