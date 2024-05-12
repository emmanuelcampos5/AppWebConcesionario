﻿using Microsoft.EntityFrameworkCore;

namespace AppWebConcesionario.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Usuario> Usuario { get; set; }

        public DbSet<Vehiculo> Vehiculo { get; set;}

    }
}
