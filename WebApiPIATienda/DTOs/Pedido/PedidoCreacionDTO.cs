using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiPIATienda.DTOs.Pedido
{
    public class PedidoCreacionDTO
    {
        public double Total { get; set; }
        public List<int> ProductosIds { get; set; }
        public List<int> Cantidades { get; set; }
        public List<double>? Subtotales { get; set; }
        [NotMapped]
        public int DireccionId { get; set; }
        [NotMapped]
        public int MetodoDePagoId { get; set; }
        public string Estado { get; set; }
    }
}
