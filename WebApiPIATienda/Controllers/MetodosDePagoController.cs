using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Xml.Linq;
using WebApiPIATienda.DTOs.MetodoDePago;
using WebApiPIATienda.Entidades;

namespace WebApiPIATienda.Controllers
{
    [ApiController]
    //[Route("usuarios/{usuarioId:int}/metodosdepago")]
    [Route("usuarios/metodosdepago")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MetodosDePagoController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public MetodosDePagoController(ApplicationDbContext dbContext, IMapper mapper,
            UserManager<IdentityUser> userManager)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetMetodoDePagoDTO>>> Get()
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

            var metodosDePago = await dbContext.MetodosDePago.Where(metodoDePagoDB => metodoDePagoDB.UsuarioId == usuarioId).ToListAsync();

            return mapper.Map<List<GetMetodoDePagoDTO>>(metodosDePago);
        }

        [HttpGet("{id:int}", Name = "obtenerMetodoDePago")]
        public async Task<ActionResult<GetMetodoDePagoDTO>> GetById(int id)
        {
            var metodoDePago = await dbContext.MetodosDePago.FirstOrDefaultAsync(metodoDePagoDB => metodoDePagoDB.Id == id);

            if (metodoDePago == null)
            {
                return NotFound();
            }

            return mapper.Map<GetMetodoDePagoDTO>(metodoDePago);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Post(MetodoDePagoCreacionDTO metodoDePagoCreacionDTO)
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

            var metodoDePago = mapper.Map<MetodoDePago>(metodoDePagoCreacionDTO);
            metodoDePago.UsuarioId = usuarioId;
            dbContext.Add(metodoDePago);
            await dbContext.SaveChangesAsync();

            var getMetodoDePagoDTO = mapper.Map<GetMetodoDePagoDTO>(metodoDePago);

            return CreatedAtRoute("obtenerMetodoDePago", new { id = metodoDePago.Id }, getMetodoDePagoDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, MetodoDePagoCreacionDTO metodoDePagoCreacionDTO)
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

            var existeMetodoDePago = await dbContext.MetodosDePago.AnyAsync(metodoDePagoDB => metodoDePagoDB.Id == id);
            if (!existeMetodoDePago)
            {
                return NotFound();
            }

            var metodoDePago = mapper.Map<MetodoDePago>(metodoDePagoCreacionDTO);
            metodoDePago.Id = id;
            metodoDePago.UsuarioId = usuarioId;

            dbContext.Update(metodoDePago);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
