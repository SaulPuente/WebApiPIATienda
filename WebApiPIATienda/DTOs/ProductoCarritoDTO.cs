using WebApiPIATienda.Entidades;

namespace WebApiPIATienda.DTOs
{
    public class ProductoCarritoDTO
    {
        public int Id { get; set; }
        public int? ProductoId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public int Cantidad { get; set; }
        public double Subtotal { get; set; }
    }
}
