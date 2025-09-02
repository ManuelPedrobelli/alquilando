using AlquileresApp.Core.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using AlquileresApp.Core.Enumerativos;
using AlquileresApp.Core.Servicios;

namespace AlquileresApp.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            if (context.Usuarios.Any())
                return;

            var hashService = new ServicioHashPassword();

            var usuarios = new List<Usuario>
            {
                new Administrador
                {
                    Nombre = "Fran",
                    Apellido = "Admin",
                    Email = "admi@gmail.com",
                    Contraseña = hashService.HashPassword("password123"),
                },
                new Cliente
                {
                    Nombre = "Milagros",
                    Apellido = "Guasco",
                    Email = "milagrosguasco11@gmail.com",
                    Telefono = "123456789",
                    Contraseña = hashService.HashPassword("password123"),
                    FechaNacimiento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                },
                new Cliente
                {
                    Nombre = "María",
                    Apellido = "García",
                    Email = "maria.garcia@test.com",
                    Telefono = "987654321",
                    Contraseña = hashService.HashPassword("password456"),
                    FechaNacimiento = new DateTime(1985, 5, 15, 0, 0, 0, DateTimeKind.Utc),
                }
            };
            context.Usuarios.AddRange(usuarios);
            context.SaveChanges();

            var propiedades = new List<Propiedad>
            {
                new Propiedad
                {
                    Titulo = "Casa en la playa",
                    Descripcion = "Hermosa casa frente al mar con vista panorámica y acceso directo a la playa",
                    Direccion = "Av. Costanera 123",
                    Localidad = "Mar del Plata",
                    PrecioPorNoche = 750.00m,
                    Capacidad = 6,
                    ServiciosDisponibles = new List<ServiciosPropiedad>
                    {
                        ServiciosPropiedad.Wifi,
                        ServiciosPropiedad.AireAcondicionado,
                        ServiciosPropiedad.Piscina,
                        ServiciosPropiedad.Estacionamiento
                    },
                    PoliticaCancelacion = PoliticasDeCancelacion.PagoTotal_48hs_50,
                    TipoPago = TipoPago.Total
                },
                new Propiedad
                {
                    Titulo = "Cabaña en la montaña",
                    Descripcion = "Acogedora cabaña con vista a la montaña y chimenea",
                    Direccion = "Cerro Catedral 456",
                    Localidad = "Bariloche",
                    PrecioPorNoche = 120.00m,
                    Capacidad = 4,
                    ServiciosDisponibles = new List<ServiciosPropiedad>
                    {
                        ServiciosPropiedad.Calefaccion,
                        ServiciosPropiedad.Estacionamiento,
                        ServiciosPropiedad.Wifi
                    },
                    PoliticaCancelacion = PoliticasDeCancelacion.Anticipo20_72hs,
                    TipoPago = TipoPago.Parcial
                }
            };
            propiedades[0].Imagenes.Add(new Imagen { Url = "/Imagenes/Propiedades/casa1.jpg" });
            propiedades[0].Imagenes.Add(new Imagen { Url = "/Imagenes/Propiedades/pileta1.jpg" });
            context.Propiedades.AddRange(propiedades);
            context.SaveChanges();

            var reservas = new List<Reserva>
            {
                new Reserva
                {
                    ClienteId = usuarios[1].Id,
                    PropiedadId = propiedades[0].Id,
                    FechaInicio = DateTime.UtcNow,
                    FechaFin = DateTime.UtcNow.AddDays(15),
                    Estado = EstadoReserva.Activa,
                    PrecioTotal = 3750,
                    MontoAPagar = 3750,
                    MontoRestante = 0,
                    TipoPago = TipoPago.Total,
                    CantidadHuespedes = 4
                },
                new Reserva
                {
                    ClienteId = usuarios[2].Id,
                    PropiedadId = propiedades[1].Id,
                    FechaInicio = DateTime.UtcNow.AddDays(5),
                    FechaFin = DateTime.UtcNow.AddDays(7),
                    Estado = EstadoReserva.Confirmada,
                    PrecioTotal = 240,
                    MontoAPagar = 240,
                    MontoRestante = 0,
                    TipoPago = TipoPago.Total,
                    CantidadHuespedes = 2
                }
            };
            context.Reservas.AddRange(reservas);
            context.SaveChanges();
        }
    }
}
