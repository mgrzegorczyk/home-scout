FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Home.Scout.Web/Home.Scout.Web.csproj", "src/Home.Scout.Web/"]
COPY ["src/Home.Scout.Application/Home.Scout.Application.csproj", "src/Home.Scout.Application/"]
COPY ["src/Home.Scout.Domain/Home.Scout.Domain.csproj", "src/Home.Scout.Domain/"]
COPY ["src/Home.Scout.Infrastructure/Home.Scout.Infrastructure.csproj", "src/Home.Scout.Infrastructure/"]
RUN dotnet restore "src/Home.Scout.Web/Home.Scout.Web.csproj"
COPY . .
WORKDIR "/src/src/Home.Scout.Web"
RUN dotnet build "./Home.Scout.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Home.Scout.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Home.Scout.Web.dll"]
