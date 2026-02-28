# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files (solution structure aware)
COPY ["src/Home.Scout.Background.Rss/Home.Scout.Background.Rss.csproj", "src/Home.Scout.Background.Rss/"]
COPY ["src/Home.Scout.Application/Home.Scout.Application.csproj", "src/Home.Scout.Application/"]
COPY ["src/Home.Scout.Infrastructure/Home.Scout.Infrastructure.csproj", "src/Home.Scout.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/Home.Scout.Background.Rss/Home.Scout.Background.Rss.csproj"

# Copy source
COPY . .

# Build
RUN dotnet build "src/Home.Scout.Background.Rss/Home.Scout.Background.Rss.csproj" -c Release -o /app/build

# Publish
RUN dotnet publish "src/Home.Scout.Background.Rss/Home.Scout.Background.Rss.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Home.Scout.Background.Rss.dll"]