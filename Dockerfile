# ============================
# 🛠️ Etapa de build
# ============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos todo el proyecto
COPY . ./

# Restauramos paquetes
RUN dotnet restore

# Publicamos en modo Release
RUN dotnet publish -c Release -o /app/publish

# ============================
# 🚀 Etapa de runtime
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiamos los artefactos publicados desde la etapa de build
COPY --from=build /app/publish ./

# Aseguramos que la carpeta 'Data' exista en el entorno de runtime
RUN mkdir -p /app/Data

# 🔁 Configura esta ruta también en tu appsettings.json:
# "ConnectionStrings": {
#   "DefaultConnection": "Data Source=Data/alquilando.db"
# }

# Iniciamos la aplicación
ENTRYPOINT ["dotnet", "AlquileresApp.UI.dll"]
