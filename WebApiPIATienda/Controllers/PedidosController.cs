using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Xml.Linq;
using WebApiPIATienda.DTOs.Pedido;
using WebApiPIATienda.Entidades;
using WebApiPIATienda.Servicios;
using Microsoft.Extensions.Logging;

namespace WebApiPIATienda.Controllers
{
    [ApiController]
    [Route("usuario/pedidos")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PedidosController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IMailService mailService;
        private readonly ILogger<PedidosController> logger;

        public PedidosController(ApplicationDbContext context, IMapper mapper,
            UserManager<IdentityUser> userManager, IMailService mailService, ILogger<PedidosController> logger)
        {
            this.dbContext = context;
            this.mapper = mapper;
            this.userManager = userManager;
            this.mailService = mailService;
            this.logger = logger;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
        [HttpGet("listadoPedidos")]
        public async Task<ActionResult<List<PedidoDTOConProductos>>> GetAll()
        {
            logger.LogInformation("Método GetAll() iniciado.");
            var pedidos = await dbContext.Pedidos
                .Include(pedidoDB => pedidoDB.ProductosPedido)
                .ThenInclude(productoPedidoDB => productoPedidoDB.Producto).ToListAsync();

            if (pedidos == null)
            {
                logger.LogWarning("No se encontraron pedidos.");
                return NotFound();
            }

            logger.LogInformation("Se encontraron {Cantidad} pedidos.", pedidos.Count);
            return mapper.Map<List<PedidoDTOConProductos>>(pedidos);
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

        [HttpGet()]
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
                .Where(x => x.UsuarioId == usuarioId).ToListAsync();

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

            var direccionV = direccion.Calle + " " + direccion.NumExt + ", " + direccion.Colonia + ", C.P." + direccion.CodigoPostal + ", " 
                + direccion.Ciudad + ", " + direccion.Estado + ", " + direccion.Pais;
            var tarjeta = metodoDePago.Bin;
            var exp = metodoDePago.Mes + "/" + metodoDePago.Año;

            //pedidoCreacionDTO.DireccionId = direccion.Id;
            //pedidoCreacionDTO.MetodoDePagoId = metodoDePago.Id;
            //pedidoCreacionDTO.Tarjeta = tarjeta;
            //pedidoCreacionDTO.Exp = exp;

            pedidoCreacionDTO.Subtotales = subtotales;
            var pedido = mapper.Map<Pedido>(pedidoCreacionDTO);

            OrdenarPorProductos(pedido);

            pedido.Total = total;

            pedido.UsuarioId = usuarioId;
            pedido.Direccion = direccionV;
            pedido.Tarjeta = tarjeta;
            pedido.Exp = exp;
            dbContext.Add(pedido);
            await dbContext.SaveChangesAsync();

            var pedidoDTO = mapper.Map<PedidoDTO>(pedido);
            

            return CreatedAtRoute("obtenerPedido", new { id = pedido.Id }, pedidoDTO);
        }

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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<PedidoPatchDTO> patchDocument)
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

            if (patchDocument == null) { return BadRequest(); }

            var pedidoDB = await dbContext.Pedidos.FirstOrDefaultAsync(x => x.Id == id);

            if (pedidoDB == null) { return NotFound(); }

            if (pedidoDB.UsuarioId != usuarioId)
            {
                return BadRequest("El pedido no pertenece al usuario.");
            }

            var pedidoDTO = mapper.Map<PedidoPatchDTO>(pedidoDB);

            patchDocument.ApplyTo(pedidoDTO);

            var isValid = TryValidateModel(pedidoDTO);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(pedidoDTO, pedidoDB);

            await dbContext.SaveChangesAsync();

            var request = new MailRequest() { ToEmail = email, Subject = $"Estado del pedido {pedidoDB.Id}", Body = $"Estado: {pedidoDTO.Estado}" };
            try
            {
                await mailService.SendEmailAsync(request);
            }
            catch (Exception ex)
            {
                throw;
            }

            return NoContent();
        }
    }
}
