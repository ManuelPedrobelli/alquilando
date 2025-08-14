# ===== Build =====
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY *.sln ./
COPY AlquileresApp.UI/*.csproj AlquileresApp.UI/
COPY AlquileresApp.Data/*.csproj AlquileresApp.Data/
COPY AlquileresApp.Core/*.csproj AlquileresApp.Core/

RUN dotnet restore
COPY . ./
WORKDIR /src/AlquileresApp.UI
RUN dotnet publish -c Release -o /app/publish

# ===== Runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    ASPNETCORE_URLS=http://+:8080

EXPOSE 8080
ENTRYPOINT ["dotnet", "AlquileresApp.UI.dll"]
