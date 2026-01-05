# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy solution and project files
COPY ["Assement.sln", "./"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Presentation/Presentation.csproj", "Presentation/"]
COPY ["TestRuleOfNegocition/TestRuleOfNegocition.csproj", "TestRuleOfNegocition/"]

# Restore dependencies
RUN dotnet restore "Assement.sln"

# Copy source code
COPY ["Domain/", "Domain/"]
COPY ["Application/", "Application/"]
COPY ["Infrastructure/", "Infrastructure/"]
COPY ["Presentation/", "Presentation/"]
COPY ["TestRuleOfNegocition/", "TestRuleOfNegocition/"]

# Build application
RUN dotnet build "Assement.sln" -c Release -o /app/build

# Publish stage
FROM build AS publish

RUN dotnet publish "Presentation/Presentation.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=publish /app/publish .

# Expose port
EXPOSE 5129

# Environment variables (can be overridden)
ENV ASPNETCORE_URLS=http://+:5129
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:5129/api/v1/course || exit 1

# Run application
ENTRYPOINT ["dotnet", "Presentation.dll"]

