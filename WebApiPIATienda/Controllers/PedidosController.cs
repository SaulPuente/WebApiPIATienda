using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using WebApiPIATienda.DTOs.MetodoDePago;
using WebApiPIATienda.DTOs.Pedido;
using WebApiPIATienda.Entidades;

namespace WebApiPIATienda.Controllers
{
    [ApiController]
    [Route("usuarios/pedidos")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PedidosController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public PedidosController(ApplicationDbContext context, IMapper mapper,
            UserManager<IdentityUser> userManager)
        {
            this.dbContext = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet]
        [HttpGet("/listadoPedidos")]
        public async Task<ActionResult<List<Pedido>>> GetAll()
        {
            return await dbContext.Pedidos.ToListAsync();
        }

        [HttpGet("{id:int}", Name = "obtenerPedido")]
        public async Task<ActionResult<PedidoDTOConProductos>> GetById(int id)
        {

            var pedido = await dbContext.Pedidos
                .Include(pedidoDB => pedidoDB.ProductosPedido)
                .ThenInclude(productoPedidoDB => productoPedidoDB.Producto)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (pedido == null)
            {
                return NotFound();
            }

            pedido.ProductosPedido = pedido.ProductosPedido.OrderBy(x => x.Orden).ToList();

            return mapper.Map<PedidoDTOConProductos>(pedido);
        }

        [HttpGet("usuario/pedidos")]
        public async Task<ActionResult<List<PedidoDTOConProductos>>> GetByUser()
        {

            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();

            var email = emailClaim.Value;

            var user = await userManager.FindByEmailAsync(email);
            var userId = user.Id;

            var usuario = await dbContext.Usuarios.FirstOrDefaultAsync(usuarioDB => usuarioDB.Email == email);

            if (usuario == null)
            {
                return NotFound();
            }

            var usuarioId = usuario.Id;

            var pedidos = await dbContext.Pedidos
                .Include(pedidoDB => pedidoDB.ProductosPedido)
                .ThenInclude(productoPedidoDB => productoPedidoDB.Producto)
                .Where(x => x.UsuarioId == usuarioId).ToListAsync(); ;

            if (pedidos == null)
            {
                return NotFound();
            }

            //var pedidosList = new List<PedidoDTOConProductos>();

            //foreach ( var pedido in pedidos)
            //{
            //    pedido.ProductosPedido = pedido.ProductosPedido.OrderBy(x => x.Orden).ToList();

            //    pedidosList.Add(mapper.Map<PedidoDTOConProductos>(pedido));
            //}

            return mapper.Map<List<PedidoDTOConProductos>>(pedidos);
        }

        [HttpPost]
        public async Task<ActionResult> Post(PedidoCreacionDTO pedidoCreacionDTO)
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();

            var email = emailClaim.Value;

            var user = await userManager.FindByEmailAsync(email);
            var userId = user.Id;

            var usuario = await dbContext.Usuarios.FirstOrDefaultAsync(usuarioDB => usuarioDB.Email == email);

            if (usuario == null)
            {
                return NotFound();
            }

            var usuarioId = usuario.Id;


            if (pedidoCreacionDTO.ProductosIds == null)
            {
                return BadRequest("No se puede crear una pedido sin productos.");
            }

            var productosIds = await dbContext.Productos
                .Where(productoBD => pedidoCreacionDTO.ProductosIds.Contains(productoBD.Id)).Select(x => x.Id).ToListAsync();

            if (pedidoCreacionDTO.ProductosIds.Count != productosIds.Count)
            {
                return BadRequest("No existe uno de los productos enviados");
            }

            var metodoDePago = await dbContext.MetodosDePago.FirstOrDefaultAsync(metodoDePagoBD => metodoDePagoBD.Id == pedidoCreacionDTO.MetodoDePagoId);
            if (metodoDePago == null)
            {
                return BadRequest("No existe el método de pago.");
            }

            var direccion = await dbContext.Direcciones.FirstOrDefaultAsync(direccionBD => direccionBD.Id == pedidoCreacionDTO.DireccionId);
            if (direccion == null)
            {
                return BadRequest("No existe la dirección.");
            }

            var total = 0.0;
            var subtotal = 0.0;
            var productoId = 0;
            var subtotales = new List<double>();

            for (int i = 0; i < pedidoCreacionDTO.ProductosIds.Count; i++)
            {
                productoId = pedidoCreacionDTO.ProductosIds[i];
                var producto = await dbContext.Productos.FirstOrDefaultAsync(productoBD => productoBD.Id == productoId);

                if (producto.Cantidad < pedidoCreacionDTO.Cantidades[i])
                {
                    return BadRequest("No hay cantidad suficiente en el inventario.");
                }

                subtotal = pedidoCreacionDTO.Cantidades[i] * producto.Precio;
                subtotales.Add(subtotal);
                total += subtotal;

                producto.Cantidad -= pedidoCreacionDTO.Cantidades[i];
                dbContext.Update(producto);
                await dbContext.SaveChangesAsync();
            }

            var direccionId = direccion.Id;
            var metodoDePagoId = metodoDePago.Id;

            pedidoCreacionDTO.MetodoDePagoId = metodoDePagoId;
            pedidoCreacionDTO.DireccionId = direccionId;

            pedidoCreacionDTO.Subtotales = subtotales;
            var pedido = mapper.Map<Pedido>(pedidoCreacionDTO);

            OrdenarPorProductos(pedido);

            pedido.Total = total;

            pedido.UsuarioId = usuarioId;
            pedido.DireccionId = direccionId;
            Console.WriteLine(direccion.Id);
            pedido.MetodoDePagoId = metodoDePagoId;
            dbContext.Add(pedido);
            await dbContext.SaveChangesAsync();

            var pedidoDTO = mapper.Map<PedidoDTO>(pedido);

            return CreatedAtRoute("obtenerPedido", new { id = pedido.Id }, pedidoDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, PedidoCreacionDTO pedidoCreacionDTO)
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();

            var email = emailClaim.Value;

            var user = await userManager.FindByEmailAsync(email);
            var userId = user.Id;

            var usuario = await dbContext.Usuarios.FirstOrDefaultAsync(usuarioDB => usuarioDB.Email == email);

            if (usuario == null)
            {
                return NotFound();
            }

            var usuarioId = usuario.Id;

            var pedidoDB = await dbContext.Pedidos
                .Include(pedidoDB => pedidoDB.ProductosPedido)
                .ThenInclude(productoPedidoDB => productoPedidoDB.Producto)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (pedidoDB == null)
            {
                return NotFound();
            }

            var metodoDePago = await dbContext.MetodosDePago.FirstOrDefaultAsync(metodoDePagoBD => metodoDePagoBD.Id == pedidoCreacionDTO.MetodoDePagoId);
            if (metodoDePago == null)
            {
                return BadRequest("No existe el método de pago.");
            }

            var direccion = await dbContext.Direcciones.FirstOrDefaultAsync(direccionBD => direccionBD.Id == pedidoCreacionDTO.DireccionId);
            if (direccion == null)
            {
                return BadRequest("No existe la dirección.");
            }

            var productoId = 0;

            for (int i = 0; i < pedidoCreacionDTO.ProductosIds.Count; i++)
            {
                productoId = pedidoCreacionDTO.ProductosIds[i];
                var producto = await dbContext.Productos.FirstOrDefaultAsync(productoBD => productoBD.Id == productoId);


                producto.Cantidad += pedidoCreacionDTO.Cantidades[i];
                dbContext.Update(producto);
                await dbContext.SaveChangesAsync();
            }

            if (pedidoCreacionDTO.ProductosIds == null)
            {
                return BadRequest("No se puede crear una pedido sin productos.");
            }

            var productosIds = await dbContext.Productos
                .Where(productoBD => pedidoCreacionDTO.ProductosIds.Contains(productoBD.Id)).Select(x => x.Id).ToListAsync();

            if (pedidoCreacionDTO.ProductosIds.Count != productosIds.Count)
            {
                return BadRequest("No existe uno de los productos enviados");
            }

            var total = 0.0;
            var subtotal = 0.0;
            productoId = 0;
            var subtotales = new List<double>();

            for (int i = 0; i < pedidoCreacionDTO.ProductosIds.Count; i++)
            {
                productoId = pedidoCreacionDTO.ProductosIds[i];
                var producto = await dbContext.Productos.FirstOrDefaultAsync(productoBD => productoBD.Id == productoId);

                if (producto.Cantidad < pedidoCreacionDTO.Cantidades[i])
                {
                    return BadRequest("No hay cantidad suficiente en el inventario.");
                }

                subtotal = pedidoCreacionDTO.Cantidades[i] * producto.Precio;
                subtotales.Add(subtotal);
                total += subtotal;

                producto.Cantidad -= pedidoCreacionDTO.Cantidades[i];
                dbContext.Update(producto);
                await dbContext.SaveChangesAsync();
            }

            pedidoCreacionDTO.Subtotales = subtotales;

            pedidoDB = mapper.Map(pedidoCreacionDTO, pedidoDB);

            OrdenarPorProductos(pedidoDB);

            pedidoDB.Total = total;
            pedidoDB.DireccionId = direccion.Id;
            pedidoDB.MetodoDePagoId = metodoDePago.Id;

            //dbContext.Update(carritoDB);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        //[HttpDelete("{id:int}")]
        //public async Task<ActionResult> Delete(int id)
        //{
        //    var carrito = await dbContext.Carritos
        //        .Include(carritoDB => carritoDB.ProductosCarrito)
        //        .ThenInclude(productoCarritoDB => productoCarritoDB.Producto)
        //        .FirstOrDefaultAsync(x => x.Id == id);

        //    if (carrito == null)
        //    {
        //        return NotFound("El Recurso no fue encontrado.");
        //    }

        //    //var validateRelation = await dbContext.AlumnoClase.AnyAsync

        //    //var productosCarritoIds = await dbContext.ProductosCarritos
        //    //    .Where(productoCarritoBD => productoCarritoBD.CarritoId == id).Select(x => x.Id).ToListAsync();

        //    //foreach (var productoCarritoId in productosCarritoIds) 
        //    //{
        //    //    dbContext.Remove(new ProductoCarrito() { Id = productoCarritoId });
        //    //    await dbContext.SaveChangesAsync();
        //    //}

        //    dbContext.Remove(new Carrito() { Id = id });
        //    await dbContext.SaveChangesAsync();

        //    return Ok();
        //}

        private void OrdenarPorProductos(Pedido pedido)
        {
            if (pedido.ProductosPedido != null)
            {
                for (int i = 0; i < pedido.ProductosPedido.Count; i++)
                {
                    pedido.ProductosPedido[i].Orden = i;
                }
            }
        }

        //[HttpPatch("{id:int}")]
        //public async Task<ActionResult> Patch(int id, JsonPatchDocument<ClasePatchDTO> patchDocument)
        //{
        //    if (patchDocument == null) { return BadRequest(); }

        //    var claseDB = await dbContext.Clases.FirstOrDefaultAsync(x => x.Id == id);

        //    if (claseDB == null) { return NotFound(); }

        //    var claseDTO = mapper.Map<ClasePatchDTO>(claseDB);

        //    patchDocument.ApplyTo(claseDTO);

        //    var isValid = TryValidateModel(claseDTO);

        //    if (!isValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    mapper.Map(claseDTO, claseDB);

        //    await dbContext.SaveChangesAsync();
        //    return NoContent();
        //}
    }
}
