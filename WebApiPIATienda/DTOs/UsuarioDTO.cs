using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using WebApiPIATienda.Entidades;
using WebApiPIATienda.Validaciones;

namespace WebApiPIATienda.DTOs
{
    public class UsuarioDTO
    {
        [EmailAddress]
        [Required(ErrorMessage = "El campo {0} es requerido")] //
        [StringLength(maximumLength: 100, ErrorMessage = "El campo {0} solo puede tener hasta 100 caracteres")]
        public string Email { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")] //
        [StringLength(maximumLength: 100, ErrorMessage = "El campo {0} solo puede tener hasta 100 caracteres")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")] //
        [StringLength(maximumLength: 100, ErrorMessage = "El campo {0} solo puede tener hasta 100 caracteres")]
        [PrimeraLetraMayuscula]
        public string Apellidos { get; set; }

        [StringLength(maximumLength: 10, ErrorMessage = "El campo {0} debe contener 10 caracteres", MinimumLength = 10)]
        [EsNumerico]
        public string? Telefono { get; set; }
        public DateTime FechaCreacion { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
