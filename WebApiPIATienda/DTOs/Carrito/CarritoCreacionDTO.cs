using System.ComponentModel.DataAnnotations;
using WebApiPIATienda.Validaciones;

namespace WebApiPIATienda.DTOs.Carrito
{
    public class CarritoCreacionDTO
    {
        public double Total { get; set; }
        public List<int> ProductosIds { get; set; }
        public List<int> Cantidades { get; set; }
        public List<double>? Subtotales { get; set; }
    }
}
