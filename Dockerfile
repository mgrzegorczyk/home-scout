# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file
COPY ["src/Home.Scout.Web/Home.Scout.Web.csproj", "src/Home.Scout.Web/"]

# Restore dependencies
RUN dotnet restore "src/Home.Scout.Web/Home.Scout.Web.csproj"

# Copy all source code
COPY . .

# Build application
RUN dotnet build "src/Home.Scout.Web/Home.Scout.Web.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "src/Home.Scout.Web/Home.Scout.Web.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:8080/Health || exit 1

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Home.Scout.Web.dll"]

