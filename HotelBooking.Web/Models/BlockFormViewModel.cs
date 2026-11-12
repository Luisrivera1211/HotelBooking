using System.ComponentModel.DataAnnotations;
using HotelBooking.Business.DTOs.Block;

namespace HotelBooking.Web.Models
{
    
    public class BlockFormViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar 50 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un hotel.")]
        public int HotelId { get; set; }

        public CreateBlockDto ToDto() => new()
        {
                Name = Name,
                HotelId = HotelId,
        };
    }
}
