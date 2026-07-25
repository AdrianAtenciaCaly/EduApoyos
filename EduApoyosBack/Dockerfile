FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copiar archivos de proyecto
COPY src/EduApoyos.Domain/EduApoyos.Domain.csproj src/EduApoyos.Domain/
COPY src/EduApoyos.Application/EduApoyos.Application.csproj src/EduApoyos.Application/
COPY src/EduApoyos.Infrastructure/EduApoyos.Infrastructure.csproj src/EduApoyos.Infrastructure/
COPY src/EduApoyos.API/EduApoyos.API.csproj src/EduApoyos.API/

# Restaurar dependencias dentro del contenedor Linux
RUN dotnet restore src/EduApoyos.API/EduApoyos.API.csproj

# Copiar código fuente
COPY src/ src/

# Compilar y publicar
WORKDIR /src/src/EduApoyos.API

RUN dotnet publish EduApoyos.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore


# ==========================================
# Runtime
# ==========================================

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

# Crear usuario no root
RUN adduser --disabled-password --gecos "" appuser \
    && chown -R appuser /app

USER appuser

# Copiar publicación
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 8080

ENTRYPOINT ["dotnet", "EduApoyos.API.dll"]