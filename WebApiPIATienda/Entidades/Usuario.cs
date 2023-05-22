using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using WebApiPIATienda.Validaciones;

namespace WebApiPIATienda.Entidades
{
    public class Usuario
    {
        public int Id { get; set; }

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

        [StringLength(maximumLength: 10, ErrorMessage = "El campo {0} solo puede tener hasta 10 caracteres")]
        [EsNumerico]
        public string? Telefono { get; set; }
        public DateTime? FechaCreacion { get; set; }

        public List<MetodoDePago>? MetodosDePago { get; set; }
        public List<Carrito>? CarritoProductos { get; set; }
        public List<Direccion>? Direcciones { get; set; }
        public List<Pedido>? Pedidos { get; set; }
    }
}
