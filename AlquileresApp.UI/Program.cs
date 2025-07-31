using Microsoft.EntityFrameworkCore;
using AlquileresApp.Core.Entidades;
using AlquileresApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Crear ruta a la base de datos
var dbFolder = Path.Combine(AppContext.BaseDirectory, "Data");
Directory.CreateDirectory(dbFolder); // Asegura que la carpeta exista
var dbPath = Path.Combine(dbFolder, "alquilando.db");

// Configurar la conexión a SQLite
builder.Configuration["ConnectionStrings:DefaultConnection"] = $"Data Source={dbPath}";

// Agregar servicios
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<AlquileresDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<DbContext, AlquileresDbContext>();
builder.Services.AddScoped<DbContextInitializer>();

var app = builder.Build();

// Inicializar la base de datos
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DbContextInitializer>();
    await initializer.InitializeAsync();
}

// Configurar el pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
