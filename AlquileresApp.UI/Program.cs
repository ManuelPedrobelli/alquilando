using AlquileresApp.UI.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using AlquileresApp.Data;
using AlquileresApp.Core.CasosDeUso.Administrador;
using AlquileresApp.Core.CasosDeUso.Calificacion;
using AlquileresApp.Core.CasosDeUso.Comentario;
using AlquileresApp.Core.CasosDeUso.ContactarAdmin;
using AlquileresApp.Core.CasosDeUso.Imagen;
using AlquileresApp.Core.CasosDeUso.PreguntasFrecuentes;
using AlquileresApp.Core.CasosDeUso.Promocion;
using AlquileresApp.Core.CasosDeUso.Propiedad;
using AlquileresApp.Core.CasosDeUso.Reserva;
using AlquileresApp.Core.CasosDeUso.Tarjeta;
using AlquileresApp.Core.CasosDeUso.Usuario;

using AlquileresApp.Core.Interfaces;
using AlquileresApp.Core.Validadores;
using AlquileresApp.Core.Servicios;
using Microsoft.EntityFrameworkCore;
using AlquileresApp.Core.Entidades;
using AlquileresApp.Core;
using Microsoft.AspNetCore.Components.Authorization;

using Npgsql.EntityFrameworkCore.PostgreSQL;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
{
    // Toma la conexión de la variable de entorno CONNECTION_STRING
    var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
    options.UseNpgsql(connectionString);
});

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddServerSideBlazor(options =>
{
    options.DetailedErrors = true;
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
});
builder.Services.AddCascadingAuthenticationState();

// --- Autenticación ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Logout";
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
builder.Services.AddAuthorizationCore();

// --- Inyecciones de servicios propios ---
builder.Services.AddScoped<IServicioHashPassword, ServicioHashPassword>();
builder.Services.AddScoped<ICalificacionRepositorio, CalificacionRepositorio>();
builder.Services.AddScoped<IComentarioRepositorio, ComentarioRepositorio>();
builder.Services.AddScoped<IPropiedadRepositorio, PropiedadesRepositorio>();
builder.Services.AddScoped<IImagenesRepositorio, ImagenesRepositorio>();
builder.Services.AddScoped<IPreguntasFrecuentesRepositorio, PreguntasFrecuentesRepositorio>();
builder.Services.AddScoped<IPromocionRepositorio, PromocionRepositorio>();
builder.Services.AddScoped<IPropiedadRepositorio, PropiedadesRepositorio>();
builder.Services.AddScoped<IReservaRepositorio, ReservaRepositorio>();
builder.Services.AddScoped<ITarjetaRepositorio, TarjetaRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<IUsuarioValidador, UsuarioValidador>();
builder.Services.AddScoped<INotificadorEmail>(sp => 
    new NotificadorEmail(
        "tuemail@gmail.com",   // remitente
        "tuclave",             // clave de la app o contraseña
        "smtp.gmail.com",      // servidor opcional
        587                    // puerto opcional
    )
);

//VALIDADORES
builder.Services.AddScoped<IPropiedadValidador, PropiedadValidador>();
builder.Services.AddScoped<IFechaReservaValidador, FechaReservaValidador>();
builder.Services.AddScoped<ITarjetaValidador, TarjetaValidador>();
builder.Services.AddScoped<ServicioAutenticacion>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<ServicioAutenticacion>());
builder.Services.AddScoped<ServicioSesion>();
builder.Services.AddScoped<IServicioAutenticacion, ServicioAutenticacion>();
builder.Services.AddScoped<IServicioSesion, ServicioSesion>();
builder.Services.AddScoped<ServicioCookies>();
builder.Services.AddScoped<CasoDeUsoCerrarSesion>();
builder.Services.AddScoped<CasoDeUsoListarPropiedadesDestacadas>();


//CU ADMINISTRADOR
builder.Services.AddScoped<CasoDeUsoEliminarEncargado>();
builder.Services.AddScoped<CasoDeUsoListarEncargados>();
builder.Services.AddScoped<CasoDeUsoRegistrarEncargado>();

//CU CALIFICACION
builder.Services.AddScoped<CasoDeUsoAgregarCalificacion>();
builder.Services.AddScoped<CasoDeUsoMostrarCalificacion>();

//CU COMENTARIO
builder.Services.AddScoped<CasoDeUsoAgregarComentario>();
builder.Services.AddScoped<CasoDeUsoListarComentarios>();
builder.Services.AddScoped<CasoDeUsoOcultarComentario>();

//CU CONTACTAR ADMIN
builder.Services.AddScoped<CasoDeUsoContactarAdmin>();

//CU IMAGEN
builder.Services.AddScoped<CasoDeUsoCargarImagen>();
builder.Services.AddScoped<CasoDeUsoEliminarImagen>();
builder.Services.AddScoped<CasoDeUsoMostrarImagenes>();

//CU PREGUNTAS FRECUENTES
builder.Services.AddScoped<CasoDeUsoCrearPreguntaFrecuente>();
builder.Services.AddScoped<CasoDeUsoEliminarPreguntaFrecuente>();
builder.Services.AddScoped<CasoDeUsoModificarPreguntaFrecuente>();
builder.Services.AddScoped<CasoDeUsoMostrarPreguntasFrecuentes>();

//CU PROMOCION
builder.Services.AddScoped<CasoDeUsoCrearPromocion>();
builder.Services.AddScoped<CasoDeUsoEliminarPromocion>();
builder.Services.AddScoped<CasoDeUsoListarPromociones>();
builder.Services.AddScoped<CasoDeUsoListarPromocionesActivas>();
builder.Services.AddScoped<CasoDeUsoModificarPromocion>();
builder.Services.AddScoped<CasoDeUsoObtenerPromocion>();

//CU PROPIEDAD
builder.Services.AddScoped<CasoDeUsoAgregarPropiedad>();
builder.Services.AddScoped<CasoDeUsoCalcularPrecioConPromocion>();
builder.Services.AddScoped<CasoDeUsoEliminarPropiedad>();
builder.Services.AddScoped<CasoDeUsoListarPropiedadesFiltrado>();
builder.Services.AddScoped<CasoDeUsoListarPropiedad>();
builder.Services.AddScoped<CasoDeUsoListarPropiedades>();
builder.Services.AddScoped<CasoDeUsoListarPropiedadesDestacadas>();
builder.Services.AddScoped<CasoDeUsoModificarPropiedad>();
builder.Services.AddScoped<CasoDeUsoMarcarPropiedadComoNoHabitable>();
builder.Services.AddScoped<CasoDeUsoObtenerPropiedad>();
builder.Services.AddScoped<CasoDeUsoObtenerPropiedades>();

//CU RESERVA
builder.Services.AddScoped<CasoDeUsoCancelarReserva>();
builder.Services.AddScoped<CasoDeUsoCrearReserva>();
builder.Services.AddScoped<CasoDeUsoInformarCheckOut>();
builder.Services.AddScoped<CasoDeUsoListarMisReservas>();
builder.Services.AddScoped<CasoDeUsoListarReservasAdm>();
builder.Services.AddScoped<CasoDeUsoModificarReserva>();
builder.Services.AddScoped<CasoDeUsoObtenerReserva>();
builder.Services.AddScoped<CasoDeUsoRegistrarCheckout>();
builder.Services.AddScoped<CasoDeUsoVerReserva>();
builder.Services.AddScoped<CasoDeUsoVisualizarReserva>();

//CU TARJETA
builder.Services.AddScoped<CasoDeUsoEliminarTarjeta>();
builder.Services.AddScoped<CasoDeUsoModificarTarjeta>();
builder.Services.AddScoped<CasoDeUsoPagar>();
builder.Services.AddScoped<CasoDeUsoRegistrarTarjeta>();
builder.Services.AddScoped<CasoDeUsoVisualizarTarjeta>();

//CU USUARIO
builder.Services.AddScoped<CasoDeUsoIniciarSesion>();
builder.Services.AddScoped<CasoDeUsoCerrarSesion>();
builder.Services.AddScoped<CasoDeUsoRegistrarUsuario>();



// Agregá el resto de tus servicios e inyecciones igual que antes
// ...

builder.Services.AddResponseCompression();

var app = builder.Build();
app.UseResponseCompression();

// --- Crear la base de datos si no existe ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
}
/*
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate(); // Ejecuta migraciones automáticamente
}*/

// --- Middleware ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorPages();
app.MapControllers();
app.MapBlazorHub();

app.MapGet("/Logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});

app.MapFallbackToPage("/_Host");

app.Run();