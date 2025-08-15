using AlquileresApp.Core.Entidades;
using AlquileresApp.Core.Enumerativos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace AlquileresApp.Data
{
    public class AppDbContext : DbContext
    {
        // Constructor para inyección de dependencias
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Encargado> Encargados { get; set; }
        public DbSet<Propiedad> Propiedades { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Tarjeta> Tarjetas { get; set; }
        public DbSet<Imagen> Imagenes { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<Calificacion> Calificaciones { get; set; }
        public DbSet<Promocion> Promociones { get; set; }
        public DbSet<PreguntaFrecuente> PreguntasFrecuentes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración TPH
            modelBuilder.Entity<Usuario>()
                .HasDiscriminator(u => u.Rol)
                .HasValue<Cliente>(RolUsuario.Cliente)
                .HasValue<Administrador>(RolUsuario.Administrador)
                .HasValue<Encargado>(RolUsuario.Encargado);

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Propiedad>()
                .Property(p => p.Localidad)
                .IsRequired();

            modelBuilder.Entity<Propiedad>()
                .Property(p => p.TipoPago)
                .IsRequired();

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Cliente)
                .WithMany(c => c.Reservas)
                .HasForeignKey(r => r.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Promocion>()
               .HasMany(p => p.Propiedades)
               .WithMany(p => p.Promociones);

            modelBuilder.Entity<Promocion>(entity =>
            {
                entity.Property(p => p.FechaInicio).HasColumnType("timestamptz");
                entity.Property(p => p.FechaFin).HasColumnType("timestamptz");
                entity.Property(p => p.FechaInicioReserva).HasColumnType("timestamptz");
                entity.Property(p => p.FechaFinReserva).HasColumnType("timestamptz");
            });

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Propiedad)
                .WithMany(p => p.Reservas)
                .HasForeignKey(r => r.PropiedadId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tarjeta>()
                .HasOne<Cliente>()
                .WithMany()
                .HasForeignKey(t => t.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Propiedad>()
                .HasMany(p => p.Comentarios)
                .WithOne(c => c.Propiedad)
                .HasForeignKey(c => c.PropiedadId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.ComentariosRealizados)
                .WithOne(c => c.Usuario)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Calificacion>()
                .HasOne(c => c.Propiedad)
                .WithMany(p => p.Calificaciones)
                .HasForeignKey(c => c.PropiedadId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Calificacion>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.CalificacionesRealizadas)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
