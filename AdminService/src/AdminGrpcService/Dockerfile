# Base image for gRPC service
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .

# Set working directory to the project folder
WORKDIR /src/AdminGrpcService

# Restore using the actual .csproj file
RUN dotnet restore AdminGrpcService.csproj

# Publish the project
RUN dotnet publish AdminGrpcService.csproj -c Release -o /app

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "AdminGrpcService.dll"]