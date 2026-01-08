# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY CleanArthitecture_Microservices.sln ./

# Copy AbstractionBlocks projects
COPY AbstractionBlocks/Common/Common.csproj AbstractionBlocks/Common/
COPY AbstractionBlocks/Common.Domain/Common.Domain.csproj AbstractionBlocks/Common.Domain/
COPY AbstractionBlocks/Common.Application/Common.Application.csproj AbstractionBlocks/Common.Application/
COPY AbstractionBlocks/Common.Infrastructure/Common.Infrastructure.csproj AbstractionBlocks/Common.Infrastructure/
COPY AbstractionBlocks/Common.Exception/Common.Exception.csproj AbstractionBlocks/Common.Exception/
COPY AbstractionBlocks/Common.Authentication/Common.Authentication.csproj AbstractionBlocks/Common.Authentication/
COPY AbstractionBlocks/Common.SecretBase/Common.SecretBase.csproj AbstractionBlocks/Common.SecretBase/
COPY AbstractionBlocks/Common.Validation/Common.Validation.csproj AbstractionBlocks/Common.Validation/
COPY AbstractionBlocks/Common.Messaging/Common.Messaging.csproj AbstractionBlocks/Common.Messaging/

# Copy ProductService projects
COPY src/ProductService/Product.Domain/Product.Domain.csproj src/ProductService/Product.Domain/
COPY src/ProductService/Product.Application/Product.Application.csproj src/ProductService/Product.Application/
COPY src/ProductService/Product.Infrastructure/Product.Infrastructure.csproj src/ProductService/Product.Infrastructure/
COPY src/ProductService/Product.Api/Product.Api.csproj src/ProductService/Product.Api/

# Restore dependencies
RUN dotnet restore src/ProductService/Product.Api/Product.Api.csproj

# Copy everything else
COPY AbstractionBlocks/ AbstractionBlocks/
COPY src/ProductService/ src/ProductService/

# Build
WORKDIR /src/src/ProductService/Product.Api
RUN dotnet build Product.Api.csproj -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish Product.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Product.Api.dll"]
