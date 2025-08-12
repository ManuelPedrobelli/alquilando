# ===== Etapa de build =====
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-stage
WORKDIR /src

# Copiar archivos de solución y proyectos
COPY *.sln ./
COPY AlquileresApp.UI/*.csproj AlquileresApp.UI/
COPY AlquileresApp.Data/*.csproj AlquileresApp.Data/
COPY AlquileresApp.Core/*.csproj AlquileresApp.Core/

# Restaurar dependencias
RUN dotnet restore

# Copiar el resto del código
COPY . ./

# Publicar en Release
WORKDIR /src/AlquileresApp.UI
RUN dotnet publish -c Release -o /app/publish

# ===== Etapa de runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build-stage /app/publish ./


ENTRYPOINT ["dotnet", "AlquileresApp.UI.dll"]
