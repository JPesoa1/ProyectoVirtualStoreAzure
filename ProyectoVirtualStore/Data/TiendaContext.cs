using Microsoft.EntityFrameworkCore;
using ProyectoNugetVirtualStore.Models;

namespace ProyectoVirtualStore.Data

{
    public class TiendaContext:DbContext
    {
        public TiendaContext(DbContextOptions<TiendaContext> options) : base(options) { }

        public DbSet<Juegos> Juegos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Comentarios> Comentarios{ get; set; }
        public DbSet<VistaComentarios> VistaComentarios{ get; set; }

        public DbSet<Categorias> Categorias{ get; set; }

        public DbSet<Compra> Compras { get; set; }
        public DbSet<CompraJuego> CompraJuegos { get; set; }

        public DbSet<Imagenes> Imagenes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CompraJuego>()
                .HasKey(cj => new { cj.IdCompra, cj.IdJuego });
        }



    }
}
