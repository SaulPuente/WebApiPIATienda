using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPIATienda.DTOs;
using WebApiPIATienda.Entidades;

namespace WebApiPIATienda.Controllers
{
    [ApiController]
    [Route("tiendas")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class TiendasController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public TiendasController(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            this.dbContext = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        //[HttpGet("configuraciones")]
        //public ActionResult<string> ObtenerConfiguracion()
        //{
        //    var configDirecta = configuration["apellido"];
        //    var configMapeada = configuration["connectionStrings:defaultConnection"];

        //    Console.WriteLine("Se obtiene valor de configuracion directo: " + configDirecta);
        //    Console.WriteLine("Se obtiene valor de configuracion mappeado: " + configMapeada);
        //    return Ok();
        //}

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<GetTiendaDTO>>> Get()
        {
            var tiendas = await dbContext.Tiendas.ToListAsync();
            return mapper.Map<List<GetTiendaDTO>>(tiendas);
        }


        //[HttpGet("{id:int}", Name = "obteneralumno")] //Se puede usar ? para que no sea obligatorio el parametro /{param=Gustavo}  getAlumno/{id:int}/
        //public async Task<ActionResult<AlumnoDTOConClases>> Get(int id)
        //{
        //    var alumno = await dbContext.Alumnos
        //        .Include(alumnoDB => alumnoDB.AlumnoClase)
        //        .ThenInclude(alumnoClaseDB => alumnoClaseDB.Clase)
        //        .FirstOrDefaultAsync(alumnoBD => alumnoBD.Id == id);

        //    if (alumno == null)
        //    {
        //        return NotFound();
        //    }

        //    return mapper.Map<AlumnoDTOConClases>(alumno);

        //}

        [HttpGet("{id:int}", Name = "obtenertienda")] //Se puede usar ? para que no sea obligatorio el parametro /{param=Gustavo}  getAlumno/{id:int}/
        public async Task<ActionResult<GetTiendaDTO>> Get(int id)
        {
            var tienda = await dbContext.Tiendas.FirstOrDefaultAsync(tiendaBD => tiendaBD.Id == id);
            //.Include(tiendaoDB => tiendaoDB.AlumnoClase)
            //.ThenInclude(alumnoClaseDB => alumnoClaseDB.Clase)
            //.FirstOrDefaultAsync(alumnoBD => alumnoBD.Id == id);

            if (tienda == null)
            {
                return NotFound();
            }

            return mapper.Map<GetTiendaDTO>(tienda);

        }

        //[HttpGet("{nombre}")]
        //public async Task<ActionResult<List<GetAlumnoDTO>>> Get([FromRoute] string nombre)
        //{
        //    var alumnos = await dbContext.Alumnos.Where(alumnoBD => alumnoBD.Nombre.Contains(nombre)).ToListAsync();

        //    return mapper.Map<List<GetAlumnoDTO>>(alumnos);

        //}

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] TiendaDTO tiendaDto)
        {
            //Ejemplo para validar desde el controlador con la BD con ayuda del dbContext

            var existeTiendaMismoNombre = await dbContext.Tiendas.AnyAsync(x => x.Nombre == tiendaDto.Nombre);

            if (existeTiendaMismoNombre)
            {
                return BadRequest($"Ya existe una tienda con el nombre {tiendaDto.Nombre}");
            }

            var tienda = mapper.Map<Tienda>(tiendaDto);

            dbContext.Add(tienda);
            await dbContext.SaveChangesAsync();

            var tiendaDTO = mapper.Map<GetTiendaDTO>(tienda);
           
            return CreatedAtRoute("obtenertienda", new { id = tienda.Id }, tiendaDTO);
        }

        //[HttpPut("{id:int}")] // api/alumnos/1
        //public async Task<ActionResult> Put(AlumnoDTO alumnoCreacionDTO, int id)
        //{
        //    var exist = await dbContext.Alumnos.AnyAsync(x => x.Id == id);
        //    if (!exist)
        //    {
        //        return NotFound();
        //    }

        //    var alumno = mapper.Map<Alumno>(alumnoCreacionDTO);
        //    alumno.Id = id;

        //    dbContext.Update(alumno);
        //    await dbContext.SaveChangesAsync();
        //    return NoContent();
        //}

        //[HttpDelete("{id:int}")]
        //public async Task<ActionResult> Delete(int id)
        //{
        //    var exist = await dbContext.Alumnos.AnyAsync(x => x.Id == id);
        //    if (!exist)
        //    {
        //        return NotFound("El Recurso no fue encontrado.");
        //    }

        //    dbContext.Remove(new Alumno()
        //    {
        //        Id = id
        //    });
        //    await dbContext.SaveChangesAsync();
        //    return Ok();
        //}
    }
}
