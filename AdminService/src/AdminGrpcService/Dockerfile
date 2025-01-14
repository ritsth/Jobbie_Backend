# Use a base image for .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /src
EXPOSE 80

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/AdminGrpcService/AdminGrpcService.csproj src/AdminGrpcService/
RUN dotnet restore "src/AdminGrpcService/AdminGrpcService.csproj"
COPY . .

RUN dotnet build "src/AdminGrpcService/AdminGrpcService.csproj" -c Release -o /src/build/grpcservice

FROM build AS publish
RUN dotnet publish "src/AdminGrpcService/AdminGrpcService.csproj" -c Release -o /src/publish/grpcservice

# Copy the built application from the publish stage
FROM base AS final
WORKDIR /src
COPY --from=publish /src/publish/grpcservice .
ENTRYPOINT ["dotnet", "AdminGrpcService.dll"]


