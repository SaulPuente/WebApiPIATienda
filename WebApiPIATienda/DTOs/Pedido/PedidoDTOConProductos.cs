namespace WebApiPIATienda.DTOs.Pedido
{
    public class PedidoDTOConProductos : PedidoDTO
    {
        public List<ProductoPedidoDTO> Productos { get; set; }
    }
}
