FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY EduApoyos.sln ./
COPY src/EduApoyos.Domain/EduApoyos.Domain.csproj src/EduApoyos.Domain/
COPY src/EduApoyos.Application/EduApoyos.Application.csproj src/EduApoyos.Application/
COPY src/EduApoyos.Infrastructure/EduApoyos.Infrastructure.csproj src/EduApoyos.Infrastructure/
COPY src/EduApoyos.API/EduApoyos.API.csproj src/EduApoyos.API/
RUN dotnet restore src/EduApoyos.API/EduApoyos.API.csproj
COPY src/ src/
WORKDIR /src/src/EduApoyos.API
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 8080
ENTRYPOINT ["dotnet", "EduApoyos.API.dll"]