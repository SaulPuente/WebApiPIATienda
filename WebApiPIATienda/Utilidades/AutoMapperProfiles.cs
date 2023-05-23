using AutoMapper;
using WebApiPIATienda.DTOs;
using WebApiPIATienda.DTOs.MetodoDePago;
using WebApiPIATienda.DTOs.Carrito;
using WebApiPIATienda.Entidades;
using WebApiPIATienda.DTOs.Direccion;
using WebApiPIATienda.DTOs.Pedido;

namespace WebApiPIATienda.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<TiendaDTO, Tienda>();
            CreateMap<Tienda, GetTiendaDTO>();

            CreateMap<UsuarioDTO, Usuario>();

            CreateMap<MetodoDePagoDTO, MetodoDePago>();
            CreateMap<MetodoDePago, GetMetodoDePagoDTO>();
            CreateMap<MetodoDePagoCreacionDTO, MetodoDePago>();

            CreateMap<DireccionDTO, Direccion>();
            CreateMap<Direccion, GetDireccionDTO>();
            CreateMap<DireccionCreacionDTO, Direccion>();

            //CreateMap<Producto, ProductoDTOConCarritos>()
            //    .ForMember(alumnoDTO => alumnoDTO.Clases, opciones => opciones.MapFrom(MapProductoDTOCarritos));
            CreateMap<CarritoCreacionDTO, Carrito>()
                .ForMember(carrito => carrito.ProductosCarrito, opciones => opciones.MapFrom(MapProductoCarrito));
            CreateMap<Carrito, CarritoDTO>();
            CreateMap<Carrito, CarritoDTOConProductos>()
                .ForMember(carritoDTO => carritoDTO.Productos, opciones => opciones.MapFrom(MapCarritoDTOProductos));

            CreateMap<PedidoCreacionDTO, Pedido>()
                .ForMember(pedido => pedido.ProductosPedido, opciones => opciones.MapFrom(MapProductoPedido));
            CreateMap<Pedido, PedidoDTO>();
            CreateMap<Pedido, PedidoDTOConProductos>()
                .ForMember(pedidoDTO => pedidoDTO.Productos, opciones => opciones.MapFrom(MapPedidoDTOProductos));
            CreateMap<PedidoPatchDTO, Pedido>().ReverseMap();
            //CreateMap<ClasePatchDTO, Clase>().ReverseMap();
            //CreateMap<CursoCreacionDTO, Cursos>();
            //CreateMap<Cursos, CursoDTO>();

            CreateMap<ProductoDTO, Producto>();
            CreateMap<Producto, GetProductoDTO>();
            CreateMap<ProductoPatchDTO, Producto>().ReverseMap();
            CreateMap<Producto, ProductoImagenDTO>();
        }

        //private List<CarritoDTO> MapProductoDTOCarritos(Producto producto, GetProductoDTO getProductoDTO)
        //{
        //    var result = new List<CarritoDTO>();

        //    if (producto.ProductoCarrito == null) { return result; }

        //    foreach (var productoCarrito in producto.ProductoCarrito)
        //    {
        //        result.Add(new CarritoDTO()
        //        {
        //            Id = productoCarrito.CarritoId,
        //            UsuarioId = productoCarrito.Carrito.UsuarioId,
        //            Total = productoCarrito.Carrito.Total
        //        });
        //    }

        //    return result;
        //}

        private List<ProductoCarritoDTO> MapCarritoDTOProductos(Carrito carrito, CarritoDTO carritoDTO)
        {
            var result = new List<ProductoCarritoDTO>();

            if (carrito.ProductosCarrito == null)
            {
                return result;
            }

            foreach (var carritoProducto in carrito.ProductosCarrito)
            {
                result.Add(new ProductoCarritoDTO()
                {
                    Id = carritoProducto.Id,
                    ProductoId = carritoProducto.ProductoId,
                    Nombre = carritoProducto.Producto.Nombre,
                    Descripcion = carritoProducto.Producto.Descripcion,
                    Categoria = carritoProducto.Producto.Categoria,
                    Cantidad = carritoProducto.Cantidad,
                    Subtotal = carritoProducto.Cantidad * carritoProducto.Producto.Precio,
                    ImagenURL = carritoProducto.Producto.ImagenURL
                });
            }

            return result;
        }

        private List<ProductoCarrito> MapProductoCarrito(CarritoCreacionDTO carritoCreacionDTO, Carrito carrito)
        {
            var resultado = new List<ProductoCarrito>();

            if (carritoCreacionDTO.ProductosIds == null) { return resultado; }

            for (int i = 0; i < carritoCreacionDTO.ProductosIds.Count; i++)
            {
                resultado.Add(new ProductoCarrito() 
                { 
                    ProductoId = carritoCreacionDTO.ProductosIds[i],
                    Cantidad = carritoCreacionDTO.Cantidades[i],
                    Subtotal = carritoCreacionDTO.Subtotales[i]
                });
            }
            return resultado;
        }

        private List<ProductoPedidoDTO> MapPedidoDTOProductos(Pedido pedido, PedidoDTO pedidoDTO)
        {
            var result = new List<ProductoPedidoDTO>();

            if (pedido.ProductosPedido == null)
            {
                return result;
            }

            pedido.ProductosPedido = pedido.ProductosPedido.OrderBy(x => x.Orden).ToList();

            foreach (var pedidoProducto in pedido.ProductosPedido)
            {
                result.Add(new ProductoPedidoDTO()
                {
                    Id = pedidoProducto.Id,
                    ProductoId = pedidoProducto.ProductoId,
                    Nombre = pedidoProducto.Producto.Nombre,
                    Descripcion = pedidoProducto.Producto.Descripcion,
                    Categoria = pedidoProducto.Producto.Categoria,
                    Cantidad = pedidoProducto.Cantidad,
                    Subtotal = pedidoProducto.Cantidad * pedidoProducto.Producto.Precio,
                    ImagenURL = pedidoProducto.Producto.ImagenURL
                    //Tarjeta = pedidoProducto.Tarjeta,
                    //Exp = pedidoProducto.Exp,
                    //Direccion = pedidoProducto.Direccion
                }); ;
            }

            return result;
        }

        private List<ProductoPedido> MapProductoPedido(PedidoCreacionDTO pedidoCreacionDTO, Pedido pedido)
        {
            var resultado = new List<ProductoPedido>();

            if (pedidoCreacionDTO.ProductosIds == null) { return resultado; }

            for (int i = 0; i < pedidoCreacionDTO.ProductosIds.Count; i++)
            {
                resultado.Add(new ProductoPedido()
                {
                    ProductoId = pedidoCreacionDTO.ProductosIds[i],
                    Cantidad = pedidoCreacionDTO.Cantidades[i],
                    Subtotal = pedidoCreacionDTO.Subtotales[i],
                    //DireccionId = pedidoCreacionDTO.DireccionId,
                    //MetodoDePagoId = pedidoCreacionDTO.MetodoDePagoId
                });
            }
            return resultado;
        }
    }
}