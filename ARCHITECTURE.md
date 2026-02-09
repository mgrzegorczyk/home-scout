# Architektura - Home Scout

## 🏗️ Architektura projektu

```
┌─────────────────────────────────────────────────────────┐
│                   Presentation Layer                     │
│        (ASP.NET Core Razor Pages / Bootstrap 5)         │
│  ┌──────────────────┐  ┌──────────────────────┐        │
│  │ Pages/Channels   │  │ Pages/News           │        │
│  │ - Index.cshtml   │  │ - Index.cshtml       │        │
│  │ - Create.cshtml  │  │ - Delete.cshtml      │        │
│  │ - Edit.cshtml    │  │                      │        │
│  │ - Delete.cshtml  │  │                      │        │
│  │ - Fetch.cshtml   │  │                      │        │
│  └──────────────────┘  └──────────────────────┘        │
└────────────────┬────────────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────────────┐
│              Business Logic Layer                        │
│  ┌──────────────────────────────────────────┐           │
│  │        Services/                         │           │
│  │ - IRssFeedService                        │           │
│  │ - RssFeedService                         │           │
│  │   (RSS parsing, feed synchronization)    │           │
│  └──────────────────────────────────────────┘           │
└────────────────┬────────────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────────────┐
│               Data Access Layer                          │
│  ┌────────────────┐  ┌─────────────────────┐           │
│  │ Repositories   │  │ Models              │           │
│  ├────────────────┤  ├─────────────────────┤           │
│  │ IRssChannelRep │  │ RssChannel          │           │
│  │ INewsItemRep   │  │ NewsItem            │           │
│  └────────────────┘  └─────────────────────┘           │
│      (Dapper)                                           │
└────────────────┬────────────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────────────┐
│            Data Storage (SQL Server)                     │
│  ┌──────────────────┐  ┌──────────────────┐            │
│  │ RssChannels      │  │ NewsItems        │            │
│  │ - Id             │  │ - Id             │            │
│  │ - Name           │  │ - RssChannelId   │            │
│  │ - Url            │  │ - Title          │            │
│  │ - Description    │  │ - Description    │            │
│  │ - CreatedAt      │  │ - Link           │            │
│  │ - UpdatedAt      │  │ - PublishedAt    │            │
│  │                  │  │ - CreatedAt      │            │
│  └──────────────────┘  └──────────────────┘            │
└─────────────────────────────────────────────────────────┘
```

---

## 📦 Struktura folderów

```
Home.Scout/
├── src/
│   └── Home.Scout.Web/                # ASP.NET Core aplikacja
│       ├── Models/                    # Domain models
│       │   ├── RssChannel.cs          # Model kanału RSS
│       │   └── NewsItem.cs            # Model artykułu
│       │
│       ├── Data/                      # Data Access Layer
│       │   ├── IRssChannelRepository.cs    # Interface
│       │   ├── RssChannelRepository.cs     # Implementacja (Dapper)
│       │   ├── INewsItemRepository.cs      # Interface
│       │   ├── NewsItemRepository.cs       # Implementacja (Dapper)
│       │   └── Scripts/               # Skrypty SQL
│       │       └── 001_Initial_Schema.sql # Schemat bazy
│       │
│       ├── Services/                  # Business Logic Layer
│       │   ├── IRssFeedService.cs     # Interface
│       │   └── RssFeedService.cs      # Pobieranie RSS
│       │
│       ├── Pages/                     # Presentation Layer
│       │   ├── Channels/              # Zarządzanie kanałami
│       │   │   ├── Index.cshtml       # Lista kanałów
│       │   │   ├── Create.cshtml      # Dodawanie kanału
│       │   │   ├── Edit.cshtml        # Edycja kanału
│       │   │   ├── Delete.cshtml      # Usuwanie kanału
│       │   │   ├── Fetch.cshtml       # Pobieranie artykułów
│       │   │   └── *.cshtml.cs        # Code-behind
│       │   │
│       │   ├── News/                  # Przeglądanie newsów
│       │   │   ├── Index.cshtml       # Lista newsów
│       │   │   ├── Delete.cshtml      # Usuwanie newsa
│       │   │   └── *.cshtml.cs        # Code-behind
│       │   │
│       │   ├── Shared/                # Wspólne komponenty
│       │   │   ├── _Layout.cshtml     # Master layout
│       │   │   └── _ValidationScriptsPartial.cshtml
│       │   │
│       │   ├── Index.cshtml           # Strona główna
│       │   └── Privacy.cshtml         # Privacy page
│       │
│       ├── wwwroot/                   # Pliki statyczne
│       │   ├── css/
│       │   │   └── site.css
│       │   └── js/
│       │       └── site.js
│       │
│       ├── Properties/                # Project properties
│       │   └── launchSettings.json
│       │
│       ├── appsettings.json           # Production config
│       ├── appsettings.Development.json # Dev config
│       ├── Program.cs                 # Entry point
│       └── Home.Scout.Web.csproj      # Project file
│
├── tests/                             # Unit tests (future)
│   └── Home.Scout.Tests.csproj
│
├── Dockerfile                         # Docker image definition
├── docker-compose.yml                 # Multi-container setup
├── .gitignore                         # Git ignore rules
├── .dockerignore                      # Docker ignore rules
├── Home.Scout.sln                     # Solution file
│
└── Documentation
    ├── README.md                      # Pełna dokumentacja
    ├── QUICK_START.md                 # Szybki start
    ├── INSTALLATION.md                # Instrukcje instalacji
    ├── DEVELOPMENT.md                 # Development guide
    ├── ARCHITECTURE.md                # Ten plik
    └── RSS_FEEDS_SAMPLES.md           # Przykładowe kanały RSS
```

---

## 🔄 Przepływ danych

### Dodawanie kanału RSS
```
User (Browser)
    ↓
Channels/Create.cshtml (Form)
    ↓
CreateModel.OnPostAsync()
    ↓
IRssChannelRepository.AddAsync()
    ↓
SQL Server: INSERT INTO RssChannels
    ↓
Redirect to Channels/Index
```

### Pobieranie artykułów
```
User kliknie "Pobierz"
    ↓
Channels/Fetch.cshtml.cs (OnGetAsync)
    ↓
IRssFeedService.FetchAndSaveNewsAsync()
    ↓
System.ServiceModel.Syndication (Parse RSS)
    ↓
IRssChannelRepository.GetByIdAsync() (pobrać kanał)
    ↓
INewsItemRepository.GetByChannelIdAsync() (sprawdzić duplikaty)
    ↓
INewsItemRepository.AddAsync() (wstawić nowe)
    ↓
SQL Server: INSERT INTO NewsItems
    ↓
Display results (Fetch.cshtml)
```

### Przeglądanie newsów
```
User (Browser)
    ↓
News/Index.cshtml (View)
    ↓
IndexModel.OnGetAsync()
    ↓
INewsItemRepository.GetAllAsync()
    ↓
IRssChannelRepository.GetAllAsync() (dla nazw kanałów)
    ↓
SQL Server: SELECT queries
    ↓
Display in table (Index.cshtml)
```

---

## 💾 Baza danych

### Schemat bazy

**Tabela RssChannels**
- `Id` - Klucz główny
- `Name` - Nazwa kanału
- `Url` - URL do RSS feed'a
- `Description` - Opis kanału
- `CreatedAt` - Data utworzenia
- `UpdatedAt` - Data ostatniej edycji

**Tabela NewsItems**
- `Id` - Klucz główny
- `RssChannelId` - Klucz obcy → RssChannels
- `Title` - Tytuł artykułu
- `Description` - Skrót artykułu
- `Link` - URL do pełnego artykułu
- `PublishedAt` - Data publikacji artykułu
- `CreatedAt` - Data dodania do bazy

### Indeksy
- `IX_NewsItems_RssChannelId` - Dla szybkiego wyszukiwania po kanale
- `IX_NewsItems_PublishedAt` - Dla sortowania po dacie
- `IX_RssChannels_Name` - Dla szybkiego wyszukiwania po nazwie

---

## 🛠️ Stack technologiczny

| Warstwa | Technologia | Wersja |
|---------|-------------|--------|
| **Framework** | ASP.NET Core | 9.0 |
| **Language** | C# | 12 |
| **Frontend** | Razor Pages | 9.0 |
| **CSS** | Bootstrap | 5.x |
| **Icons** | Bootstrap Icons | 1.11+ |
| **Database** | Microsoft SQL Server | 2022 |
| **ORM** | Dapper | 2.1+ |
| **RSS Parsing** | System.ServiceModel.Syndication | 4.7+ |
| **Containerization** | Docker | Latest |
| **VCS** | Git | - |

---

## 🔐 Warstwy bezpieczeństwa

```
┌─────────────────────────────────────────┐
│        ASP.NET Core Security            │
│ - HTTPS (Production)                    │
│ - CSRF Protection (built-in)            │
│ - Input Validation                      │
│ - Output Encoding                       │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│    Dapper Parameterized Queries         │
│ - SQL Injection Prevention              │
│ - Type-safe queries                     │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│      Database Security (SQL Server)     │
│ - User authentication                   │
│ - Encrypted connection                  │
│ - Access control                        │
└─────────────────────────────────────────┘
```

---

## 📈 Skalowanie

### Obecna architektura
- Single-server deployment
- Synchronous RSS fetching
- In-memory data processing

### Przyszłe ulepsz
enia dla skalowania
- Background jobs (Hangfire/Worker Service)
- Asynchronous RSS fetching
- Caching layer (Redis)
- Database replicas
- Load balancing
- Message queue (RabbitMQ)

---

## 🧪 Testing Strategy

### Unit Tests (przyszłość)
- RssFeedService tests
- Repository tests (mocked DB)
- Model validation tests

### Integration Tests (przyszłość)
- Database tests
- Full request/response tests
- Docker container tests

### Manual Testing
- Functional testing w przeglądarce
- RSS feed URL validation
- Database integrity checks

---

## 📊 Performance Considerations

### Optimizations
- Database indexes na kolumnach wyszukiwania
- Asynchronous methods (async/await)
- Parameterized queries (Dapper)
- Connection pooling (SQL Server)

### Monitoring (przyszłość)
- Application Insights
- Custom logging
- Performance counters
- Database query analysis

---

## 🚀 Deployment Strategy

### Development
- Local .NET runtime
- Local SQL Server/LocalDB
- Hot reload enabled

### Testing
- Docker containers
- Integration with CI/CD

### Production
- Docker containers (Kubernetes-ready)
- SQL Server backup strategy
- Environment variable configuration
- HTTPS enforcement
- Regular security updates

---

## 📝 Code Style Guidelines

- **Naming**: PascalCase dla publicznych, camelCase dla prywatnych
- **Comments**: XML documentation na publicznych API
- **Async**: Zawsze async dla I/O operations
- **Null-safety**: Implicit nullable reference types enabled
- **SOLID**: Dependency injection, interfaces, single responsibility

---

## 🔗 Powiązania

```
Program.cs (Entry Point)
    ├── Registers Services (DI Container)
    ├── Configures Middleware
    └── Routes to Pages
            │
            ├── Pages/Channels/* 
            │   └── IRssChannelRepository
            │   └── INewsItemRepository
            │   └── IRssFeedService
            │
            └── Pages/News/*
                └── INewsItemRepository
                └── IRssChannelRepository
```

---

## 📚 Dodatkowe zasoby

- [ASP.NET Core Docs](https://docs.microsoft.com/aspnet/core/)
- [Dapper Documentation](https://github.com/DapperLib/Dapper)
- [SQL Server Documentation](https://docs.microsoft.com/sql/sql-server/)
- [Docker Documentation](https://docs.docker.com/)
- [C# Programming Guide](https://docs.microsoft.com/dotnet/csharp/)

