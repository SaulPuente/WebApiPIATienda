using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using WebApiPIATienda.DTOs;
using WebApiPIATienda.Entidades;

namespace WebApiPIATienda.Controllers
{
    [ApiController]
    [Route("productos")]
    public class ProductosController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public ProductosController(ApplicationDbContext context, IMapper mapper)
        {
            this.dbContext = context;
            this.mapper = mapper;
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
            //.Include(tiendaoDB => tiendaoDB.AlumnoClase)
            //.ThenInclude(alumnoClaseDB => alumnoClaseDB.Clase)
            //.FirstOrDefaultAsync(alumnoBD => alumnoBD.Id == id);

            if (producto == null)
            {
                return NotFound();
            }

            return mapper.Map<GetProductoDTO>(producto);

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

            return CreatedAtRoute("obtenerproducto", new { id = producto.Id }, productoDTO);
        }

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
    }
}
