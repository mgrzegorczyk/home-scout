# Instrukcje instalacji - Home Scout RSS News Aggregator

## Szybki start (Docker) - REKOMENDOWANE

### Wymagania
- Docker Desktop (pobierz z https://www.docker.com/products/docker-desktop)
- System Windows 10/11, macOS lub Linux

### Kroki

1. **Otwórz terminal/PowerShell w katalogu projektu**
   ```bash
   cd C:\Users\<YourUsername>\Desktop\Home.Scout
   ```

2. **Zbuduj i uruchom aplikację**
   ```bash
   docker-compose up --build
   ```
   
   ⏳ Czekaj aż zobaczysz komunikaty o uruchomieniu serwisu web

3. **Otwórz aplikację w przeglądarce**
   ```
   http://localhost:8080
   ```

4. **Zatrzymanie aplikacji**
   ```bash
   docker-compose down
   ```

---

## Instalacja lokalna (.NET 9.0)

### Wymagania
- .NET 9.0 SDK (pobierz z https://dotnet.microsoft.com/download)
- SQL Server Express lub LocalDB
- Visual Studio Code, Visual Studio 2022 lub Rider

### Kroki

1. **Zainstaluj .NET 9.0 SDK**
   - Pobierz ze strony Microsoft
   - Uruchom installer i postępuj wg instrukcji
   - Sprawdź instalację:
     ```bash
     dotnet --version
     ```

2. **Przygotuj bazę danych**

   **Opcja A: Używając SQL Server Management Studio**
   - Otwórz SQL Server Management Studio
   - Połącz się z `(localdb)\mssqllocaldb` lub swoim serwerem SQL
   - Otwórz plik `src\Home.Scout.Web\Data\Scripts\001_Initial_Schema.sql`
   - Kliknij "Execute" lub naciśnij F5
   - Czekaj aż baza zostanie utworzona

   **Opcja B: Używając PowerShell**
   ```powershell
   sqlcmd -S (localdb)\mssqllocaldb -i "src\Home.Scout.Web\Data\Scripts\001_Initial_Schema.sql"
   ```

3. **Uruchom aplikację**
   ```bash
   cd src\Home.Scout.Web
   dotnet restore
   dotnet run
   ```

4. **Otwórz aplikację w przeglądarce**
   ```
   https://localhost:5001
   lub
   http://localhost:5000
   ```

---

## Troubleshooting

### Problem: "Connection string not found"
**Rozwiązanie:**
- Sprawdź czy plik `appsettings.json` zawiera `ConnectionStrings`
- Sprawdź czy baza danych "HomeScout" istnieje w SQL Server

### Problem: "Cannot connect to database"
**Rozwiązanie:**
```bash
# Sprawdź czy SQL Server jest uruchomiony
Get-Service MSSQLSERVER | Start-Service

# Lub sprawdź LocalDB
sqllocaldb start mssqllocaldb
```

### Problem: Docker się nie uruchamia
**Rozwiązanie:**
1. Upewnij się że Docker Desktop jest uruchomiony
2. Sprawdź czy porty 8080 i 1433 są dostępne:
   ```bash
   netstat -ano | findstr :8080
   netstat -ano | findstr :1433
   ```
3. Jeśli porty są zajęte, zmień je w `docker-compose.yml`

### Problem: "Port already in use"
**Rozwiązanie:**
Edytuj `docker-compose.yml` i zmień porty:
```yaml
ports:
  - "9090:8080"    # zmieniono z 8080 na 9090
  - "1444:1433"    # zmieniono z 1433 na 1444
```

### Problem: Aplikacja nie ładuje się
**Rozwiązanie:**
```bash
# Czyszczenie cache
docker-compose down -v
docker system prune -a

# Przebuduj
docker-compose up --build
```

### Problem: "NuGet packages failed to restore"
**Rozwiązanie:**
```bash
dotnet nuget locals all --clear
dotnet restore
```

---

## Pierwsze kroki w aplikacji

### 1. Dodaj kanał RSS
1. Kliknij "📡 Kanały" w menu
2. Kliknij "Dodaj kanał"
3. Wpisz:
   - **Nazwa**: np. "BBC News"
   - **URL**: np. "https://feeds.bbc.co.uk/news/rss.xml"
   - **Opis**: (opcjonalny)
4. Kliknij "Dodaj kanał"

### 2. Pobierz artykuły
1. Na stronie kanałów kliknij "Pobierz" przy wybranym kanale
2. Czekaj aż artykuły się załadują
3. Kliknij "Przejdź do wszystkich newsów"

### 3. Przeglądaj newsy
1. Kliknij "📰 Newsy" w menu
2. Zobaczysz tabelę ze wszystkimi artykułami
3. Kliknij na tytuł aby przejść do artykułu
4. Użyj przycisku "Usuń" aby usunąć artykuł

---

## Popularne kanały RSS do testowania

```
BBC News
https://feeds.bbc.co.uk/news/rss.xml

Reuters
https://www.reuters.com/finance

CNBC
https://www.cnbc.com/id/100003114/device/rss/rss.html

The Guardian
https://www.theguardian.com/world/rss

CNN
http://rss.cnn.com/rss/edition.rss
```

---

## Dalsze kroki

1. **Przeczytaj** `README.md` - pełna dokumentacja
2. **Przeczytaj** `DEVELOPMENT.md` - instrukcje dla developerów
3. **Eksploruj kod** w katalogu `src/Home.Scout.Web`

---

## Kontakt i problemy

Jeśli napotkasz problemy:
1. Sprawdź logi w Docker: `docker-compose logs`
2. Sprawdź logi w aplikacji w konsoli
3. Sprawdź czy wszystkie wymagania są spełnione

---

## Informacje o bezpieczeństwie

⚠️ **WAŻNE - Dla produkcji zmień:**
1. Hasło w `docker-compose.yml` (SA_PASSWORD)
2. Connection string w `appsettings.json`
3. Włącz HTTPS
4. Użyj environment variables

Patrz `README.md` sekcja "Bezpieczeństwo" po więcej informacji.

