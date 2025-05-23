# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

# Go to correct subdirectory
WORKDIR /src/AdminService/src/AdminGrpcService



RUN dotnet restore AdminGrpcService.csproj
RUN dotnet publish AdminGrpcService.csproj -c Release -o /app

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "AdminGrpcService.dll"]
