# Base image for .NET Grpc service
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build image for compiling .NET app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the project file and restore dependencies
COPY JobService.Grpc/JobService.Grpc.csproj ./JobService.Grpc/
RUN dotnet restore ./JobService.Grpc/JobService.Grpc.csproj

# Copy the rest of the application code
COPY . .

# Build and publish the application
WORKDIR /src/JobService.Grpc
RUN dotnet publish -c Release -o /app

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "JobService.Grpc.dll"]
