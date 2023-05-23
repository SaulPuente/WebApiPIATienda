using System.Security.Cryptography.X509Certificates;

namespace WebApiPIATienda.DTOs
{
    public class ProductoPedidoDTO
    {
        public int Id { get; set; }
        public int? ProductoId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public int Cantidad { get; set; }
        public double Subtotal { get; set; }
        public string? ImagenURL { get; set; }
        public string? Imagen { get; set; }
    }
}
