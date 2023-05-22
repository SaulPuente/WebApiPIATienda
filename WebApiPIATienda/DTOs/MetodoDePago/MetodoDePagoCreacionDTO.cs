using System.ComponentModel.DataAnnotations;
using WebApiPIATienda.Validaciones;

namespace WebApiPIATienda.DTOs.MetodoDePago
{
    public class MetodoDePagoCreacionDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 16, ErrorMessage = "El campo {0} debe contener 16 caracteres", MinimumLength = 16)]
        [EsNumerico]
        public string Bin { get; set; } = null!;
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 2, ErrorMessage = "El campo {0} solo puede tener hasta 2 caracteres", MinimumLength = 2)]
        [EsNumerico]
        public string Mes { get; set; } = null!;
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 2, ErrorMessage = "El campo {0} solo puede tener hasta 2 caracteres", MinimumLength = 2)]
        [EsNumerico]
        public string Año { get; set; } = null!;
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 10, ErrorMessage = "El campo {0} solo puede tener hasta 10 caracteres")]
        public string Tipo { get; set; } = null!;
    }
}
