# Base image for .NET API service
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build image for compiling .NET app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
WORKDIR /src/JobService.Api
RUN dotnet restore
RUN dotnet publish -c Release -o /app

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "JobService.Api.dll"]
