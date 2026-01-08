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

# Copy MailNotificationService projects
COPY src/MailNotificationService/MailNotification.Domain/MailNotification.Domain.csproj src/MailNotificationService/MailNotification.Domain/
COPY src/MailNotificationService/MailNotification.Application/MailNotification.Application.csproj src/MailNotificationService/MailNotification.Application/
COPY src/MailNotificationService/MailNotification.Infrastructure/MailNotification.Infrastructure.csproj src/MailNotificationService/MailNotification.Infrastructure/
COPY src/MailNotificationService/MailNotification.Api/MailNotification.Api.csproj src/MailNotificationService/MailNotification.Api/

# Restore dependencies
RUN dotnet restore src/MailNotificationService/MailNotification.Api/MailNotification.Api.csproj

# Copy everything else
COPY AbstractionBlocks/ AbstractionBlocks/
COPY src/MailNotificationService/ src/MailNotificationService/

# Build
WORKDIR /src/src/MailNotificationService/MailNotification.Api
RUN dotnet build MailNotification.Api.csproj -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish MailNotification.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MailNotification.Api.dll"]
