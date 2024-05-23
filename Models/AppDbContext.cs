using Microsoft.EntityFrameworkCore;
using AppWebConcesionario.Models;

namespace AppWebConcesionario.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Usuario> Usuario { get; set; }

        public DbSet<Vehiculo> Vehiculo { get; set;}

        //dylan
        public DbSet<Promocion> Promocion { get; set; }
        public DbSet<AppWebConcesionario.Models.Inventario> Inventario { get; set; } = default!;

        //dylan
    }
}
