using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using WebApiPIATienda.DTOs;
using WebApiPIATienda.DTOs.Pedido;
using WebApiPIATienda.Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;


namespace WebApiPIATienda.Controllers
{
    [ApiController]
    [Route("productos")]
    public class ProductosController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;
        private readonly ILogger<ProductosController> logger;
        private readonly IWebHostEnvironment webHostEnvironment;


        public ProductosController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager, ILogger<ProductosController> logger, IWebHostEnvironment webHostEnvironment)
        {
            this.dbContext = context;
            this.mapper = mapper;
            this.userManager = userManager;
            this.logger = logger;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<GetProductoDTO>>> Get()
        {
            var productos = await dbContext.Productos.ToListAsync();
            return mapper.Map<List<GetProductoDTO>>(productos);
        }


        [HttpGet("{id:int}", Name = "obtenerproducto")]
        public async Task<ActionResult<GetProductoDTO>> Get(int id)
        {
            var producto = await dbContext.Productos.FirstOrDefaultAsync(productoBD => productoBD.Id == id);

            if (producto == null)
            {
                return NotFound();
            }

            return mapper.Map<GetProductoDTO>(producto);

        }

        [HttpGet("imagen/{id:int}")]
        public async Task<ActionResult<ProductoImagenDTO>> GetImage(int id)
        {
            var producto = await dbContext.Productos.FirstOrDefaultAsync(productoBD => productoBD.Id == id);

            if (producto == null)
            {
                return NotFound();
            }

            return mapper.Map<ProductoImagenDTO>(producto);

        }

        [HttpGet("{nombre}")]
        public async Task<ActionResult<List<GetProductoDTO>>> Get([FromRoute] string nombre)
        {
            var productos = await dbContext.Productos.Where(productoBD => productoBD.Nombre.Contains(nombre)).ToListAsync();

            return mapper.Map<List<GetProductoDTO>>(productos);

        }

        [HttpGet("/categoria/{categoria}")]
        public async Task<ActionResult<List<GetProductoDTO>>> GetByCategory([FromRoute] string categoria)
        {
            var productos = await dbContext.Productos.Where(productoBD => productoBD.Categoria.Contains(categoria)).ToListAsync();

            return mapper.Map<List<GetProductoDTO>>(productos);

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ProductoDTO productoDto)
        {
            //Ejemplo para validar desde el controlador con la BD con ayuda del dbContext

            var existeProductoMismoNombre = await dbContext.Productos.AnyAsync(x => x.Nombre == productoDto.Nombre);

            if (existeProductoMismoNombre)
            {
                return BadRequest($"Ya existe un producto con el nombre {productoDto.Nombre}");
            }

            var producto = mapper.Map<Producto>(productoDto);

            dbContext.Add(producto);
            await dbContext.SaveChangesAsync();

            var productoDTO = mapper.Map<GetProductoDTO>(producto);

            string logFilePath = Path.Combine(webHostEnvironment.WebRootPath, "TextFile.txt");
            string logMessage = $"Registrando producto: {productoDto.Nombre}";
            
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                await writer.WriteLineAsync(logMessage);
            }

            logger.LogInformation($"Registrando producto: {productoDto.Nombre}");



            return CreatedAtRoute("obtenerproducto", new { id = producto.Id }, productoDTO);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(ProductoDTO productoDTO, int id)
        {
            var exist = await dbContext.Productos.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound();
            }

            var producto = mapper.Map<Producto>(productoDTO);
            producto.Id = id;

            dbContext.Update(producto);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await dbContext.Productos.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound("El Recurso no fue encontrado.");
            }

            dbContext.Remove(new Producto()
            {
                Id = id
            });
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<ProductoPatchDTO> patchDocument)
        {
            if (patchDocument == null) { return BadRequest(); }

            var productoDB = await dbContext.Productos.FirstOrDefaultAsync(x => x.Id == id);

            if (productoDB == null) { return NotFound(); }

            var productoDTO = mapper.Map<ProductoPatchDTO>(productoDB);

            patchDocument.ApplyTo(productoDTO);

            var isValid = TryValidateModel(productoDTO);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(productoDTO, productoDB);

            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("/recomendacion")]
        public async Task<ActionResult<List<GetProductoDTO>>> GetRecomendations()
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

            var pedidosHist = await dbContext.Pedidos.Where(x => x.UsuarioId == usuarioId).Select(x => x.Id).ToListAsync();
            var ppHist = await dbContext.ProductosPedidos.Where(x => pedidosHist.Contains(x.PedidoId)).Select(x => x.ProductoId).ToListAsync();
            var productosHist = await dbContext.Productos.Where(x => ppHist.Contains(x.Id)).Select(x => x.Id).ToListAsync();

            var productos = dbContext.Productos.Where(x => ppHist.Contains(x.Id));
            
            var productosRand = productos.OrderBy(r => Guid.NewGuid()).Take(5);

            //var productos = await dbContext.Productos.ToListAsync();

            return mapper.Map<List<GetProductoDTO>>(productosRand);
        }
    }
}
