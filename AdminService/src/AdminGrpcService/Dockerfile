# Base image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything (you are already in the correct context at build time)
COPY . .

# Set correct working directory
WORKDIR /src/AdminGrpcService

# 🔧 Restore with explicit path
RUN dotnet restore AdminGrpcService.csproj

# 🔧 Publish with explicit path
RUN dotnet publish AdminGrpcService.csproj -c Release -o /app

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "AdminGrpcService.dll"]