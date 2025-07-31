# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-stage
WORKDIR /src
COPY . ./

# Restaurar dependencias
RUN dotnet restore

# Publicar solo el proyecto principal
RUN dotnet publish AlquileresApp.UI/AlquileresApp.UI.csproj -c Release -o /app/publish

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build-stage /app/publish ./

RUN mkdir -p /app/Data
ENTRYPOINT ["dotnet", "AlquileresApp.UI.dll"]

