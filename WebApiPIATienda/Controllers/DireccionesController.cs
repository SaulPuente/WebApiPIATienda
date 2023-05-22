using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Xml.Linq;
using WebApiPIATienda.DTOs.Direccion;
using WebApiPIATienda.Entidades;

namespace WebApiPIATienda.Controllers
{
    [ApiController]
    //[Route("usuarios/{usuarioId:int}/metodosdepago")]
    [Route("usuarios/direcciones")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DireccionesController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public DireccionesController(ApplicationDbContext dbContext, IMapper mapper,
            UserManager<IdentityUser> userManager)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetDireccionDTO>>> Get()
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

            var direccion = await dbContext.Direcciones.Where(direccionDB => direccionDB.UsuarioId == usuarioId).ToListAsync();

            return mapper.Map<List<GetDireccionDTO>>(direccion);
        }

        [HttpGet("{id:int}", Name = "obtenerDireccion")]
        public async Task<ActionResult<GetDireccionDTO>> GetById(int id)
        {
            var direccion = await dbContext.Direcciones.FirstOrDefaultAsync(direccionDB => direccionDB.Id == id);

            if (direccion == null)
            {
                return NotFound();
            }

            return mapper.Map<GetDireccionDTO>(direccion);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Post(DireccionCreacionDTO direccionCreacionDTO)
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

            var direccion = mapper.Map<Direccion>(direccionCreacionDTO);
            direccion.UsuarioId = usuarioId;
            dbContext.Add(direccion);
            await dbContext.SaveChangesAsync();

            var getDireccionDTO = mapper.Map<GetDireccionDTO>(direccion);

            return CreatedAtRoute("obtenerDireccion", new { id = direccion.Id }, getDireccionDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, DireccionCreacionDTO direccionCreacionDTO)
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

            var existeDireccion = await dbContext.Direcciones.AnyAsync(direccionDB => direccionDB.Id == id);
            if (!existeDireccion)
            {
                return NotFound();
            }

            var direccion = mapper.Map<Direccion>(direccionCreacionDTO);
            direccion.Id = id;
            direccion.UsuarioId = usuarioId;

            dbContext.Update(direccion);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
