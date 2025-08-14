using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace AlquileresApp.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Toma la conexión de la variable de entorno CONNECTION_STRING
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("La variable de entorno CONNECTION_STRING no está definida.");

            optionsBuilder.UseNpgsql(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
