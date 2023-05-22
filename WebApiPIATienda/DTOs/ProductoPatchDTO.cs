using System.ComponentModel.DataAnnotations;
using WebApiPIATienda.Validaciones;

namespace WebApiPIATienda.DTOs
{
    public class ProductoPatchDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")] //
        [StringLength(maximumLength: 50, ErrorMessage = "El campo {0} solo puede tener hasta 50 caracteres")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }

        [StringLength(maximumLength: 300, ErrorMessage = "El campo {0} solo puede tener hasta 300 caracteres")]
        [PrimeraLetraMayuscula]
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public int Cantidad { get; set; }
        public double Precio { get; set; }
        public double Costo { get; set; }
    }
}
