# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar todo el proyecto
COPY . ./

# Restaurar dependencias
RUN dotnet restore

# Publicar la app (modo Release)
RUN dotnet publish -c Release -o /out

# Crear carpeta y copiar la base de datos
RUN mkdir -p /out/Data
COPY AlquileresApp.Data/alquilando.db /out/Data/alquilando.db

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copiar desde el build
COPY --from=build /out ./   

# Exponer el puerto (Render usa 10000 automáticamente)
EXPOSE 80

# Comando de arranque
ENTRYPOINT ["dotnet", "AlquileresApp.UI.dll"]
