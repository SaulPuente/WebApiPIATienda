namespace WebApiPIATienda.DTOs.Pedido
{
    public class PedidoCreacionDTO
    {
        public double Total { get; set; }
        public List<int> ProductosIds { get; set; }
        public List<int> Cantidades { get; set; }
        public List<double>? Subtotales { get; set; }
        public int DireccionId { get; set; }
        public int MetodoDePagoId { get; set; }
        public string Estado { get; set; }
    }
}
