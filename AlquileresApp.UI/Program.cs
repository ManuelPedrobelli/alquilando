using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using AlquileresApp.Data;
using AlquileresApp.Core.Servicios;
using AlquileresApp.Core.Validadores;
using AlquileresApp.Core.Interfaces;
using AlquileresApp.Core.CasosDeUso.Usuario;
using AlquileresApp.Core.CasosDeUso.Propiedad;
using AlquileresApp.Core.CasosDeUso.Reserva;
using AlquileresApp.Core.CasosDeUso.Tarjeta;
using AlquileresApp.Core.CasosDeUso.Imagen;
using AlquileresApp.Core.CasosDeUso.Comentario;
using AlquileresApp.Core.CasosDeUso.Calificacion;
using AlquileresApp.Core.CasosDeUso.Promocion;
using AlquileresApp.Core.CasosDeUso.PreguntasFrecuentes;
using AlquileresApp.Core.CasosDeUso.ContactarAdmin;
using AlquileresApp.Core;
using AlquileresApp.UI.Components;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Ruta de base de datos por defecto
var dbFolder = Path.Combine(AppContext.BaseDirectory, "Data");
Directory.CreateDirectory(dbFolder);
var dbPath = Path.Combine(dbFolder, "alquilando.db");

// Si hay variable de entorno, usar esa (ej. en Docker)
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    dbPath = "/tmp/alquilando.db";
    connectionString = $"Data Source={dbPath}";
}
builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;

// Componentes y servicios básicos
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddServerSideBlazor(options =>
{
    options.DetailedErrors = true;
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
});
builder.Services.AddCascadingAuthenticationState();

// DB Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Autenticación
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

// Inyecciones
builder.Services.AddScoped<IServicioHashPassword, ServicioHashPassword>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<IUsuarioValidador, UsuarioValidador>();
builder.Services.AddScoped<ServicioAutenticacion>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<ServicioAutenticacion>());
builder.Services.AddScoped<ServicioSesion>();
builder.Services.AddScoped<IServicioSesion, ServicioSesion>();
builder.Services.AddScoped<ServicioCookies>();
builder.Services.AddAuthorizationCore();

// Casos de uso y repositorios
builder.Services.AddScoped<CasoDeUsoRegistrarUsuario>();
builder.Services.AddScoped<CasoDeUsoIniciarSesion>();
builder.Services.AddScoped<CasoDeUsoCerrarSesion>();
builder.Services.AddScoped<IPropiedadRepositorio, PropiedadesRepositorio>();
builder.Services.AddScoped<IImagenesRepositorio, ImagenesRepositorio>();
builder.Services.AddScoped<IPropiedadValidador, PropiedadValidador>();
builder.Services.AddScoped<IReservaRepositorio, ReservaRepositorio>();
builder.Services.AddScoped<ITarjetaRepositorio, TarjetaRepositorio>();
builder.Services.AddScoped<IFechaReservaValidador, FechaReservaValidador>();
builder.Services.AddScoped<ITarjetaValidador, TarjetaValidador>();
builder.Services.AddScoped<IComentarioRepositorio, ComentarioRepositorio>();
builder.Services.AddScoped<ICalificacionRepositorio, CalificacionRepositorio>();
builder.Services.AddScoped<IPromocionRepositorio, PromocionRepositorio>();
builder.Services.AddScoped<IPreguntasFrecuentesRepositorio, PreguntaFrecuenteRepositorio>();

// Casos de uso adicionales
builder.Services.AddScoped<CasoDeUsoAgregarPropiedad>();
builder.Services.AddScoped<CasoDeUsoCrearReserva>();
builder.Services.AddScoped<CasoDeUsoRegistrarTarjeta>();
builder.Services.AddScoped<CasoDeUsoAgregarComentario>();
builder.Services.AddScoped<CasoDeUsoAgregarCalificacion>();
builder.Services.AddScoped<CasoDeUsoCrearPromocion>();
builder.Services.AddScoped<CasoDeUsoMostrarPreguntasFrecuentes>();
builder.Services.AddScoped<CasoDeUsoContactarAdmin>();

// Notificador de email
builder.Services.AddTransient<INotificadorEmail>(provider =>
    new NotificadorEmail("reservaenalquilando@gmail.com", "fxsl hsck basy pamv"));

builder.Services.AddResponseCompression();

var app = builder.Build();
app.UseResponseCompression();

// Inicializar BD
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();
        SeedData.Initialize(dbContext);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al inicializar la base de datos: {ex.Message}");
        throw;
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
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
