namespace WebApiPIATienda.Entidades
{
    public class ProductoProveedor
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public int ProveedorId { get; set; }
        public Producto Producto { get; set; }
        public Proveedor Proveedor { get; set; }
    }
}
