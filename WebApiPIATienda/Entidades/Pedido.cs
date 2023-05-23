namespace WebApiPIATienda.Entidades
{
    public class Pedido
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public List<ProductoPedido> ProductosPedido { get; set; } = null!;
        public string Tarjeta { get; set; }
        public string Exp { get; set; }
        public string Direccion { get; set; }
        public double Total { get; set; }
        public string Estado { get; set; }
    }
}
