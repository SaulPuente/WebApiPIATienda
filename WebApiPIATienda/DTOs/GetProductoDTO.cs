namespace WebApiPIATienda.DTOs
{
    public class GetProductoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public int Cantidad { get; set; }
        public double Precio { get; set; }
        public double Costo { get; set; }
        public string? ImagenURL { get; set; }
    }
}
