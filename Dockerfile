# Etap 1 – build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Kopiuj wszystko
COPY . .

# Publikacja aplikacji w trybie Release
RUN dotnet publish DevOpsDemo.Api/DevOpsDemo.Api.csproj -c Release -o out

# Etap 2 – runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Dodaj curl (wymagane dla HEALTHCHECK)
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/out .

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "DevOpsDemo.Api.dll"]
