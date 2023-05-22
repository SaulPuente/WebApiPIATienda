using System.ComponentModel.DataAnnotations;
using WebApiPIATienda.Validaciones;

namespace WebApiPIATienda.DTOs.MetodoDePago
{
    public class GetMetodoDePagoDTO
    {
        public int Id { get; set; }
        public string Bin { get; set; } = null!;
        public string Mes { get; set; } = null!;
        public string Año { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public int UsuarioId { get; set; }
    }
}
