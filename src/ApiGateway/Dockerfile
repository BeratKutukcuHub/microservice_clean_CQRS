# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY CleanArthitecture_Microservices.sln ./

# Copy ApiGateway project
COPY src/ApiGateway/ApiGateway.csproj src/ApiGateway/

# Restore dependencies
RUN dotnet restore src/ApiGateway/ApiGateway.csproj

# Copy everything else
COPY src/ApiGateway/ src/ApiGateway/

# Build
WORKDIR /src/src/ApiGateway
RUN dotnet build ApiGateway.csproj -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish ApiGateway.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

# Use Docker-specific ocelot configuration
RUN rm -f ocelot.json && mv ocelot.Docker.json ocelot.json

ENTRYPOINT ["dotnet", "ApiGateway.dll"]
