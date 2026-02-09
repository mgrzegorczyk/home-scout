# Instrukcje do lokalnego rozwoju

## Wymagania

- .NET 9.0 SDK
- SQL Server Express lub LocalDB
- Visual Studio 2022, JetBrains Rider lub Visual Studio Code

## Uruchomienie lokalne (bez Docker)

### 1. Przygotowanie bazy danych

#### Opcja A: Używając SQL Server Management Studio

1. Otwórz SQL Server Management Studio
2. Połącz się z lokalnym serwerem SQL
3. Uruchom skrypt: `src/Home.Scout.Web/Data/Scripts/001_Initial_Schema.sql`

#### Opcja B: Używając PowerShell/Command Line

```bash
sqlcmd -S (localdb)\mssqllocaldb -i "src\Home.Scout.Web\Data\Scripts\001_Initial_Schema.sql"
```

### 2. Uruchomienie aplikacji

```bash
# Przejdź do katalogu projektu
cd src/Home.Scout.Web

# Przywróć zależności
dotnet restore

# Uruchom aplikację
dotnet run
```

Aplikacja będzie dostępna pod adresem: `http://localhost:5000` (lub https://localhost:5001)

## Uruchomienie z Docker

### 1. Budowanie i uruchomienie

```bash
docker-compose up --build
```

### 2. Dostęp do aplikacji

- Aplikacja web: http://localhost:8080
- MSSQL Database: localhost,1433
  - Username: sa
  - Password: YourPassword123!

### 3. Zatrzymanie kontenerów

```bash
docker-compose down
```

### 4. Usunięcie wszystkich danych (łącznie z bazą)

```bash
docker-compose down -v
```

## Zmiana hasła do bazy danych

⚠️ **Nie zapomnij zmienić hasła w produkcji!**

1. Edytuj `docker-compose.yml` - zmień `SA_PASSWORD`
2. Edytuj `src/Home.Scout.Web/appsettings.json` - zmień connection string
3. Przebuduj kontenery: `docker-compose up --build`

## Struktura bazy danych

Baza danych jest inicjalizowana automatycznie przy pierwszym uruchomieniu z pliku:
```
src/Home.Scout.Web/Data/Scripts/001_Initial_Schema.sql
```

Jeśli chcesz ręcznie zaktualizować schemat, stwórz nowy plik:
```
src/Home.Scout.Web/Data/Scripts/002_Your_Changes.sql
```

I uruchom go w kontenerze:
```bash
docker exec home-scout-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourPassword123!" -i /docker-entrypoint-initdb.d/002_Your_Changes.sql
```

## Debugowanie

### Visual Studio 2022

1. Otwórz `Home.Scout.sln`
2. Ustaw punkt przerwania (breakpoint)
3. Naciśnij F5 lub Debug → Start Debugging
4. Aplikacja uruchomi się w debug mode

### Visual Studio Code

1. Zainstaluj rozszerzenie C# (omnisharp)
2. Otwórz folder projektu
3. Naciśnij F5 aby uruchomić debug

### JetBrains Rider

1. Otwórz `Home.Scout.sln`
2. Ustaw punkt przerwania
3. Naciśnij Shift+F9 lub Debug → Debug
4. Aplikacja uruchomi się w debug mode

## Testy

### Uruchomienie testów

```bash
dotnet test
```

### Testy integracyjne

Testy integracyjne wymagają działającej bazy danych. Upewnij się, że `docker-compose up` jest uruchomiony.

## Czyszczenie projektu

```bash
# Usunięcie wyników kompilacji
dotnet clean

# Usunięcie cache NuGet
dotnet nuget locals all --clear

# Usunięcie katalogu bin i obj
Remove-Item -Path "src/Home.Scout.Web/bin" -Recurse -Force
Remove-Item -Path "src/Home.Scout.Web/obj" -Recurse -Force
```

## Przydatne komendy Docker

```bash
# Pokaż logi aplikacji
docker-compose logs -f web

# Pokaż logi bazy danych
docker-compose logs -f db

# Połączenie do bazy danych
docker exec -it home-scout-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourPassword123!"

# Pokaż status kontenerów
docker-compose ps

# Rebuild bez cache
docker-compose build --no-cache
```

## Rozwiązywanie problemów

### Problem: "Connection string 'DefaultConnection' not found"

Upewnij się, że:
- Edytowałeś `appsettings.json` lub `appsettings.Development.json`
- Wartość connection string jest poprawna
- Baza danych jest dostępna

### Problem: "Cannot open database 'HomeScout'"

Uruchom skrypt SQL do tworzenia bazy:
```bash
sqlcmd -S (localdb)\mssqllocaldb -i "src\Home.Scout.Web\Data\Scripts\001_Initial_Schema.sql"
```

### Problem: Docker nie uruchamia się

1. Sprawdź czy Docker Desktop jest uruchomiony
2. Sprawdź logi: `docker-compose logs`
3. Przebuduj: `docker-compose build --no-cache`
4. Sprawdź czy porty 8080 i 1433 są dostępne

### Problem: NuGet packages nie pobierają się

```bash
# Wyczyść cache NuGet
dotnet nuget locals all --clear

# Spróbuj ponownie
dotnet restore
```

## Publikowanie

### Publish dla produkcji (Release)

```bash
dotnet publish -c Release -o ./publish
```

### Docker Image dla produkcji

```bash
docker build -t home-scout:latest .
docker run -p 8080:8080 home-scout:latest
```

## Więcej informacji

- [ASP.NET Core Documentation](https://learn.microsoft.com/aspnet/core)
- [Dapper GitHub](https://github.com/DapperLib/Dapper)
- [SQL Server in Docker](https://learn.microsoft.com/sql/linux/sql-server-linux-docker-container-deployment)

