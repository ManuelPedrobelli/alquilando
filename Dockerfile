# ===== Etapa de build =====
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-stage
WORKDIR /src

# Copiar archivos de solución y proyectos primero para aprovechar el caché
COPY *.sln ./
COPY AlquileresApp.UI/*.csproj AlquileresApp.UI/
COPY AlquileresApp.Data/*.csproj AlquileresApp.Data/
COPY AlquileresApp.Core/*.csproj AlquileresApp.Core/

# Restaurar dependencias
RUN dotnet restore

# Copiar el resto del código
COPY . ./

# Publicar en modo Release
WORKDIR /src/AlquileresApp.UI
RUN dotnet publish -c Release -o /app/publish

# ===== Etapa de runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Variables de entorno recomendadas
ENV DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    ASPNETCORE_URLS=http://+:8080

# Copiar los archivos publicados
COPY --from=build-stage /app/publish .

# Puerto para Render
EXPOSE 8080

ENTRYPOINT ["dotnet", "AlquileresApp.UI.dll"]
