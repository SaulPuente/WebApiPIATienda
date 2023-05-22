namespace WebApiPIATienda.Entidades
{
    public class ProductoCarrito
    {
        public int Id { get; set; }
        public int? ProductoId { get; set; }
        public int? CarritoId { get; set; }
        public Producto Producto { get; set; }
        public Carrito Carrito { get; set; }
        public int Cantidad { get; set; }
        public double Subtotal { get; set; }
        public int Orden { get; set; }
    }
}
