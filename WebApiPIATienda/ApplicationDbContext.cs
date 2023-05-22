using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiPIATienda.Entidades;

namespace WebApiPIATienda
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<AlumnoClase>()
        //        .HasKey(al => new { al.AlumnoId, al.ClaseId });
        //}
        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            foreach (var relationship in modelbuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelbuilder);
        }

        public DbSet<Tienda> Tiendas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<MetodoDePago> MetodosDePago { get; set; }
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<ProductoCarrito> ProductosCarritos { get; set; }
        public DbSet<Direccion> Direcciones { get; set; }
        public DbSet<ProductoPedido> ProductosPedidos { get; set; }
        public DbSet<Proveedor> Proveedores { get; set;}
        public DbSet<ProductoProveedor> ProductosProveedors { get; set; }
    }
}
