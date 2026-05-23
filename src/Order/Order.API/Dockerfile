FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/Shared/Shared.Contracts/Shared.Contracts.csproj src/Shared/Shared.Contracts/
COPY src/Order/Order.Domain/Order.Domain.csproj src/Order/Order.Domain/
COPY src/Order/Order.Application/Order.Application.csproj src/Order/Order.Application/
COPY src/Order/Order.Infrastructure/Order.Infrastructure.csproj src/Order/Order.Infrastructure/
COPY src/Order/Order.API/Order.API.csproj src/Order/Order.API/
RUN dotnet restore src/Order/Order.API/Order.API.csproj
COPY . .
RUN dotnet publish src/Order/Order.API/Order.API.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Order.API.dll"]
