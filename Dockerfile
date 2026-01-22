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
COPY --from=build /app/out .

# Port
EXPOSE 8080

# Start aplikacji
ENTRYPOINT ["dotnet", "DevOpsDemo.Api.dll"]
