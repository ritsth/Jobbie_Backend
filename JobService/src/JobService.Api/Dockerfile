# Base image for .NET API service
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build image for compiling .NET app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

# Go to correct subdirectory
WORKDIR /src/JobService/src/JobService.Api

# Build and publish the application
RUN dotnet restore JobService.Api.csproj
RUN dotnet publish JobService.Api.csproj -c Release -o /app

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "JobService.Api.dll"]
