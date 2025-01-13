# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /src

# Copy the csproj and restore any dependencies (via NuGet)
COPY ./src/AdminService/AdminService.csproj ./src/AdminService/
RUN dotnet restore ./src/AdminService/AdminService.csproj

# Copy the entire source code and publish it to the /out directory
COPY ./src/AdminService/ ./src/AdminService/
RUN dotnet publish ./src/AdminService/AdminService.csproj -c Release -o /out

# Use the official .NET runtime image for the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Set the working directory
WORKDIR /app

# Copy the published app from the build image
COPY --from=build /out .

# Expose the application port
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "AdminService.dll"]

