namespace WebApiPIATienda.Entidades
{
    public class ProductoPedido
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public int PedidoId { get; set; }
        public Producto Producto { get; set; }
        public Pedido Pedido { get; set; }
        public int Cantidad { get; set; }
        public double Subtotal { get; set; }
        public int Orden { get; set; }
        public int DireccionId { get; set; }
        public int MetodoDePagoId { get; set; }
        public Direccion Direccion { get; set; }
        public MetodoDePago MetodoDePago { get; set; }
    }
}
