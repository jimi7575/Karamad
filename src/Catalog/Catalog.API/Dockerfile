FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/Shared/Shared.Contracts/Shared.Contracts.csproj src/Shared/Shared.Contracts/
COPY src/Catalog/Catalog.Domain/Catalog.Domain.csproj src/Catalog/Catalog.Domain/
COPY src/Catalog/Catalog.Application/Catalog.Application.csproj src/Catalog/Catalog.Application/
COPY src/Catalog/Catalog.Infrastructure/Catalog.Infrastructure.csproj src/Catalog/Catalog.Infrastructure/
COPY src/Catalog/Catalog.API/Catalog.API.csproj src/Catalog/Catalog.API/
RUN dotnet restore src/Catalog/Catalog.API/Catalog.API.csproj
COPY . .
RUN dotnet publish src/Catalog/Catalog.API/Catalog.API.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Catalog.API.dll"]
