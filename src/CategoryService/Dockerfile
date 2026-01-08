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

# Copy CategoryService projects
COPY src/CategoryService/Category.Domain/Category.Domain.csproj src/CategoryService/Category.Domain/
COPY src/CategoryService/Category.Application/Category.Application.csproj src/CategoryService/Category.Application/
COPY src/CategoryService/Category.Infrastructure/Category.Infrastructure.csproj src/CategoryService/Category.Infrastructure/
COPY src/CategoryService/Category.Api/Category.Api.csproj src/CategoryService/Category.Api/

# Restore dependencies
RUN dotnet restore src/CategoryService/Category.Api/Category.Api.csproj

# Copy everything else
COPY AbstractionBlocks/ AbstractionBlocks/
COPY src/CategoryService/ src/CategoryService/

# Build
WORKDIR /src/src/CategoryService/Category.Api
RUN dotnet build Category.Api.csproj -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish Category.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Category.Api.dll"]
