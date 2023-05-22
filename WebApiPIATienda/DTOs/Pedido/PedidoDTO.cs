namespace WebApiPIATienda.DTOs.Pedido
{
    public class PedidoDTO
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public double Total { get; set; }
        public int DireccionId { get; set; }
        public int MetodoDePagoId { get; set; }
        public string Estado { get; set; }
        public string? Direccion { get; set; }
        public string? Tarjeta { get; set; }
        public string? Exp { get; set; }
    }
}
