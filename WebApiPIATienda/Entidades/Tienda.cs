using System.ComponentModel.DataAnnotations;
using WebApiPIATienda.Validaciones;

namespace WebApiPIATienda.Entidades
{
    public class Tienda
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")] //
        [StringLength(maximumLength: 50, ErrorMessage = "El campo {0} solo puede tener hasta 50 caracteres")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }

        [StringLength(maximumLength: 300, ErrorMessage = "El campo {0} solo puede tener hasta 300 caracteres")]
        [PrimeraLetraMayuscula]
        public string? Descripcion { get; set; }
        [StringLength(maximumLength: 10, ErrorMessage = "El campo {0} solo puede tener hasta 10 caracteres")]
        [EsNumerico]
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Horario { get; set; }
    }
}
