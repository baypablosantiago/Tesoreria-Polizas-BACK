# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia y restaura dependencias
COPY *.csproj ./
RUN dotnet restore

# Copia el resto del código
COPY . ./

# Publica en modo Release
RUN dotnet publish -c Release -o out

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/out ./

# Puerto expuesto (Render escucha en 80)
EXPOSE 80

# Comando de arranque
ENTRYPOINT ["dotnet", "Tesoreria-BACK.dll"]
