# Imagen base de .NET SDK para construir el proyecto
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copiar los archivos del proyecto y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar el resto del código y compilar
COPY . ./
RUN dotnet publish -c Release -o out

# Imagen base de .NET Runtime para ejecutar la API
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out .

# Configurar variable de entorno para el puerto en Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Comando para ejecutar la API
CMD ["dotnet", "TU_PROYECTO.dll"]
