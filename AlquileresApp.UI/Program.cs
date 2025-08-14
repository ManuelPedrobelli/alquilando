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
using Microsoft.AspNetCore.Authentication;


var builder = WebApplication.CreateBuilder(args);

// --- Configuración de la cadena de conexión ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? Environment.GetEnvironmentVariable("DATABASE_URL");

// Convertir el formato de Render (postgres://user:pass@host:5432/db) a Npgsql
if (!string.IsNullOrEmpty(connectionString) && connectionString.StartsWith("postgres://"))
{
    var uri = new Uri(connectionString);
    var userInfo = uri.UserInfo.Split(':');
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SslMode=Require;Trust Server Certificate=true";
}

// --- Configuración de servicios ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

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
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<IUsuarioValidador, UsuarioValidador>();
builder.Services.AddScoped<ServicioAutenticacion>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<ServicioAutenticacion>());
builder.Services.AddScoped<ServicioSesion>();
builder.Services.AddScoped<IServicioSesion, ServicioSesion>();
builder.Services.AddScoped<ServicioCookies>();

// Agregá el resto de tus servicios e inyecciones igual que antes
// ...

builder.Services.AddResponseCompression();

var app = builder.Build();
app.UseResponseCompression();

// --- Crear la base de datos si no existe ---
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate(); // Ejecuta migraciones automáticamente
}

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