# Home Scout - RSS News Aggregator

Aplikacja do agregacji i przeglądania newsów z kanałów RSS.

## Funkcjonalności

- 📰 **Przeglądanie newsów** - Tabela ze wszystkimi pobranymi artykułami (data+godzina | tytuł | opis | źródło)
- 📡 **Zarządzanie kanałami RSS** - Dodawanie, edycja i usuwanie kanałów
- 🔄 **Automatyczne pobieranie** - Pobieranie nowych artykułów z kanałów
- 🗄️ **Baza danych** - MSSQL z DB First pattern
- 🐳 **Docker** - Pełna konteneryzacja aplikacji

## Wymagania

- Docker i Docker Compose
- .NET 9.0 (do lokalnego rozwoju bez Dockera)

## Szybki start z Docker

1. **Klonowanie repozytorium**
   ```bash
   cd Home.Scout
   ```

2. **Uruchomienie aplikacji**
   ```bash
   docker-compose up --build
   ```

3. **Dostęp do aplikacji**
   - Aplikacja web: http://localhost:8080
   - MSSQL Database: `localhost,1433` (sa / YourPassword123!)

## Struktura projektu

```
Home.Scout/
├── src/
│   └── Home.Scout.Web/           # ASP.NET Core Razor Pages aplikacja
│       ├── Models/               # Modele danych (RssChannel, NewsItem)
│       ├── Data/                 # Warstwa dostępu do danych (repozytoria, skrypty SQL)
│       ├── Services/             # Serwisy biznesowe (RssFeedService)
│       ├── Pages/
│       │   ├── Channels/         # Strony zarządzania kanałami
│       │   ├── News/             # Strony wyświetlania newsów
│       │   └── ...
│       ├── wwwroot/              # Pliki statyczne
│       └── ...
├── tests/                         # Testy (przyszła implementacja)
├── Dockerfile                     # Dockerfile dla aplikacji
├── docker-compose.yml            # Konfiguracja Docker Compose
├── .gitignore                     # Git ignore rules
└── Home.Scout.sln                # Solution file
```

## Technologia

- **Framework**: ASP.NET Core 9.0 (Razor Pages)
- **Baza danych**: Microsoft SQL Server 2022
- **ORM**: Dapper
- **RSS Parsing**: System.ServiceModel.Syndication
- **Frontend**: Bootstrap 5

## Użytkowanie

### 1. Dodawanie kanału RSS

1. Przejdź do sekcji "Kanały" w menu nawigacji
2. Kliknij "Dodaj kanał"
3. Wypełnij formularz:
   - **Nazwa** - nazwa kanału
   - **URL** - URL do RSS feed'u
   - **Opis** - opcjonalny opis
4. Kliknij "Dodaj kanał"

### 2. Pobieranie newsów

1. Na stronie kanałów kliknij przycisk "Pobierz" przy wybranym kanale
2. Artykuły zostaną automatycznie pobrane i dodane do bazy

### 3. Przeglądanie newsów

1. Przejdź do sekcji "Newsy" w menu nawigacji
2. Zobaczysz tabelę ze wszystkimi pobranymi artykułami
3. Możesz kliknąć na tytuł, aby przejść do artykułu na stronie źródła
4. Możesz usunąć artykuł używając przycisku "Usuń"

### 4. Edycja kanału

1. Na stronie kanałów kliknij przycisk "Edytuj" przy wybranym kanale
2. Zmień dane kanału
3. Kliknij "Zaktualizuj kanał"

### 5. Usuwanie kanału

1. Na stronie kanałów kliknij przycisk "Usuń" przy wybranym kanale
2. Potwierdź usunięcie
3. Wszystkie artykuły z tego kanału zostaną również usunięte

## Baza danych

### Schemat

#### Tabela RssChannels
```sql
- Id (INT, Primary Key, Identity)
- Name (NVARCHAR(255), NOT NULL)
- Url (NVARCHAR(MAX), NOT NULL)
- Description (NVARCHAR(MAX))
- CreatedAt (DATETIME2, DEFAULT GETUTCDATE())
- UpdatedAt (DATETIME2, DEFAULT GETUTCDATE())
```

#### Tabela NewsItems
```sql
- Id (INT, Primary Key, Identity)
- RssChannelId (INT, Foreign Key → RssChannels.Id, ON DELETE CASCADE)
- Title (NVARCHAR(MAX), NOT NULL)
- Description (NVARCHAR(MAX))
- Link (NVARCHAR(MAX))
- PublishedAt (DATETIME2, NOT NULL)
- CreatedAt (DATETIME2, DEFAULT GETUTCDATE())
```

## Bazy danych w Docker

Baza danych jest inicjalizowana automatycznie przy pierwszym uruchomieniu. Skrypty SQL znajdują się w:
```
src/Home.Scout.Web/Data/Scripts/
```

## Logowanie

Aplikacja loguje do konsoli. Błędy przy pobieraniu feedów RSS są automatycznie zalogowywane.

## Bezpieczeństwo

⚠️ **Dla produkcji:**
- Zmień hasło do bazy danych w `docker-compose.yml`
- Zmień hasło w `appsettings.json`
- Włącz HTTPS
- Użyj environment variables do przechowywania wrażliwych danych

## Przyszłe ulepszenia

- [ ] Background job do automatycznego pobierania newsów co X minut
- [ ] Filtrowanie i wyszukiwanie newsów
- [ ] Sortowanie newsów
- [ ] Podział newsów na strony
- [ ] User authentication
- [ ] Kategoryzacja kanałów
- [ ] Testy jednostkowe
- [ ] Testy integracyjne

## Licencja

MIT

## Autor

GitHub Copilot

