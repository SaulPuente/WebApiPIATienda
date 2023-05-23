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
        //public string Direccion { get; set; }
        //public string Tarjeta { get; set; }
        //public string Exp { get; set; }
    }
}
