namespace WebApiPIATienda.Entidades
{
    public class Pedido
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public List<ProductoPedido> ProductosPedido { get; set; } = null!;
        public int MetodoDePagoId { get; set; }
        public MetodoDePago MetodoDePago { get; set; } = null!;
        public int DireccionId { get; set; }
        public Direccion Direccion { get; set; } = null!;
        public double Total { get; set; }
        public string Estado { get; set; }
    }
}
