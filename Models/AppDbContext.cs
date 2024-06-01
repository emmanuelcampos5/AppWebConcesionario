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

        public DbSet<Factura> Factura { get; set;}

        public DbSet<Det_Factura> Det_Factura { get; set;}
        
        public DbSet<Promocion> Promocion { get; set; }

        public DbSet<Inventario> Inventario { get; set; }

        public DbSet<RegistroAuditoria> RegistroAuditoria { get; set; }

   
    }
}
