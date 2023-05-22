using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiPIATienda.DTOs;
using WebApiPIATienda.DTOs.Carrito;
using WebApiPIATienda.Entidades;

namespace WebApiPIATienda.Controllers
{
    [ApiController]
    [Route("usuarios/carrito")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CarritosController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public CarritosController(ApplicationDbContext context, IMapper mapper,
            UserManager<IdentityUser> userManager)
        {
            this.dbContext = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet]
        [HttpGet("/listadoCarrito")]
        public async Task<ActionResult<List<Carrito>>> GetAll()
        {
            return await dbContext.Carritos.ToListAsync();
        }

        [HttpGet("{id:int}", Name = "obtenerCarrito")]
        public async Task<ActionResult<CarritoDTOConProductos>> GetById(int id)
        {

            var carrito = await dbContext.Carritos
                .Include(carritoDB => carritoDB.ProductosCarrito)
                .ThenInclude(productoCarritoDB => productoCarritoDB.Producto)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (carrito == null)
            {
                return NotFound();
            }

            carrito.ProductosCarrito = carrito.ProductosCarrito.OrderBy(x => x.Orden).ToList();

            return mapper.Map<CarritoDTOConProductos>(carrito);
        }

        [HttpGet("usuario/Carrito")]
        public async Task<ActionResult<CarritoDTOConProductos>> GetByUser()
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

            var carrito = await dbContext.Carritos
                .Include(carritoDB => carritoDB.ProductosCarrito)
                .ThenInclude(productoCarritoDB => productoCarritoDB.Producto)
                .FirstOrDefaultAsync(x => x.UsuarioId == usuarioId);

            if (carrito == null)
            {
                return NotFound();
            }

            carrito.ProductosCarrito = carrito.ProductosCarrito.OrderBy(x => x.Orden).ToList();

            return mapper.Map<CarritoDTOConProductos>(carrito);
        }

        [HttpPost]
        public async Task<ActionResult> Post(CarritoCreacionDTO carritoCreacionDTO)
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

            var existeCarritoMismoUsuario = await dbContext.Carritos.AnyAsync(x => x.UsuarioId == usuarioId);

            if (existeCarritoMismoUsuario)
            {
                return BadRequest($"Ya existe un carrito para el usuario {email}");
            }

            if (carritoCreacionDTO.ProductosIds == null)
            {
                return BadRequest("No se puede crear una carrito sin productos.");
            }

            var productosIds = await dbContext.Productos
                .Where(productoBD => carritoCreacionDTO.ProductosIds.Contains(productoBD.Id)).Select(x => x.Id).ToListAsync();

            if (carritoCreacionDTO.ProductosIds.Count != productosIds.Count)
            {
                return BadRequest("No existe uno de los productos enviados");
            }

            var total = 0.0;
            var subtotal = 0.0;
            var productoId = 0;
            var subtotales = new List<double>();

            for (int i = 0; i < carritoCreacionDTO.ProductosIds.Count; i++)
            {
                productoId = carritoCreacionDTO.ProductosIds[i];
                var producto = await dbContext.Productos.FirstOrDefaultAsync(productoBD => productoBD.Id == productoId);

                subtotal = carritoCreacionDTO.Cantidades[i] * producto.Precio;
                subtotales.Add(subtotal);
                total += subtotal;

                //producto.Cantidad -= carritoCreacionDTO.Cantidades[i];
                //dbContext.Update(producto);
                //await dbContext.SaveChangesAsync();
            }

            carritoCreacionDTO.Subtotales = subtotales;
            var carrito = mapper.Map<Carrito>(carritoCreacionDTO);

            OrdenarPorProductos(carrito);

            carrito.Total = total;

            carrito.UsuarioId = usuarioId;
            dbContext.Add(carrito);
            await dbContext.SaveChangesAsync();

            var carritoDTO = mapper.Map<CarritoDTO>(carrito);

            return CreatedAtRoute("obtenerCarrito", new { id = carrito.Id }, carritoDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, CarritoCreacionDTO carritoCreacionDTO)
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

            var carritoDB =  await dbContext.Carritos
                .Include(carritoDB => carritoDB.ProductosCarrito)
                .ThenInclude(productoCarritoDB => productoCarritoDB.Producto)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (carritoDB == null)
            {
                return NotFound();
            }

            if (carritoCreacionDTO.ProductosIds == null)
            {
                return BadRequest("No se puede crear una carrito sin productos.");
            }

            var productosIds = await dbContext.Productos
                .Where(productoBD => carritoCreacionDTO.ProductosIds.Contains(productoBD.Id)).Select(x => x.Id).ToListAsync();

            if (carritoCreacionDTO.ProductosIds.Count != productosIds.Count)
            {
                return BadRequest("No existe uno de los productos enviados");
            }

            var total = 0.0;
            var subtotal = 0.0;
            var productoId = 0;
            var subtotales = new List<double>();

            for (int i = 0; i < carritoCreacionDTO.ProductosIds.Count; i++)
            {
                productoId = carritoCreacionDTO.ProductosIds[i];
                var producto = await dbContext.Productos.FirstOrDefaultAsync(productoBD => productoBD.Id == productoId);

                subtotal = carritoCreacionDTO.Cantidades[i] * producto.Precio;
                subtotales.Add(subtotal);
                total += subtotal;

                //producto.Cantidad -= carritoCreacionDTO.Cantidades[i];
                //dbContext.Update(producto);
                //await dbContext.SaveChangesAsync();
            }

            carritoCreacionDTO.Subtotales = subtotales;

            carritoDB = mapper.Map(carritoCreacionDTO, carritoDB);

            OrdenarPorProductos(carritoDB);

            carritoDB.Total = total;

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

        private void OrdenarPorProductos(Carrito carrito)
        {
            if (carrito.ProductosCarrito != null)
            {
                for (int i = 0; i < carrito.ProductosCarrito.Count; i++)
                {
                    carrito.ProductosCarrito[i].Orden = i;
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
