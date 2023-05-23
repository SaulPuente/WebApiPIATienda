namespace WebApiPIATienda.DTOs.Pedido
{
    public class PedidoPatchDTO
    {
        public double Total { get; set; }
        public string Estado { get; set; }
        public string? Direccion { get; set; }
        public string? Tarjeta { get; set; }
        public string? Exp { get; set; }
    }
}
