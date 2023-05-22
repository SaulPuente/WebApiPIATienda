using System.ComponentModel.DataAnnotations;
using WebApiPIATienda.Validaciones;

namespace WebApiPIATienda.Entidades
{
    public class Carrito
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public List<ProductoCarrito> ProductosCarrito { get; set; }
        public double Total { get; set; }
    }
}
