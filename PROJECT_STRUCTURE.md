# 📁 Project Structure - Complete File Map

## Overview
```
Home.Scout/
├── 📄 Documentation (8 files)
├── 🔧 Configuration (4 files)  
├── 💻 Source Code (1 folder)
├── 🗂️ Tests (future)
└── 🔑 Repository Files
```

---

## 📄 Documentation Files

### 📑 INDEX.md
**Purpose**: Navigation hub for all documentation
- Decision tree for which doc to read
- Common scenarios with workflows
- Quick reference table
- Pro tips

### 🚀 GETTING_STARTED.md
**Purpose**: First-time user guide
- Super quick start (2 minutes)
- Step-by-step first steps
- Sample RSS feeds to test with
- Quick troubleshooting

### ⚡ QUICK_START.md
**Purpose**: Ultra-fast startup instructions
- One-liner commands
- Minimal setup required
- Links to detailed docs

### 📋 INSTALLATION.md
**Purpose**: Detailed installation guide
- System requirements
- Step-by-step instructions
- Docker setup
- Local .NET setup
- Extensive troubleshooting section
- Command references

### 📖 README.md
**Purpose**: Complete project documentation
- Feature overview
- Technology stack
- Usage guide
- Database schema
- Security best practices
- Future improvements

### 🔧 DEVELOPMENT.md
**Purpose**: Developer guide
- Local development setup
- Database setup instructions
- Docker debugging
- Testing strategy
- Environment configuration
- Useful commands
- Troubleshooting for developers

### 🏗️ ARCHITECTURE.md
**Purpose**: System design and architecture
- Architecture diagrams (ASCII art)
- Data flow diagrams
- Folder structure with descriptions
- Technology stack details
- Database schema details
- Code organization patterns
- Performance considerations
- Future scaling strategy

### 📡 RSS_FEEDS_SAMPLES.md
**Purpose**: Example RSS feed URLs
- Categorized feed samples (News, Tech, Business, etc.)
- How to find more feeds
- Testing recommendations
- Feed validation tips

### ✅ SUMMARY.md
**Purpose**: Project completion summary
- Checklist of completed requirements
- List of all created files
- Features implemented
- Technology stack used
- Quick start instructions
- Future improvements

### 📋 TODO.md
**Purpose**: Future roadmap and improvements
- Completed features (v1.0.0)
- Phase 2 improvements (High Priority)
- Phase 3 enhancements
- Phase 4 advanced features
- Testing & quality assurance
- Security improvements
- DevOps & deployment
- Roadmap timeline
- Priority matrix

### 📁 PROJECT_STRUCTURE.md
**Purpose**: This file - complete file map
- Detailed file listing
- File descriptions
- Line counts and sizes
- Dependencies between files

---

## 🔧 Configuration Files

### docker-compose.yml (49 lines)
**Purpose**: Multi-container Docker configuration
**Contains**:
- MSSQL 2022 service configuration
- ASP.NET Core web service configuration
- Volume management
- Network setup
- Health checks
- Environment variables

### Dockerfile (30 lines)
**Purpose**: Docker image definition for web app
**Contains**:
- Multi-stage build (SDK → publish → runtime)
- Dependencies installation
- Application build and publish
- Health check
- Port configuration
- Environment setup

### .dockerignore (27 lines)
**Purpose**: Files to exclude from Docker build
**Contains**: bin/, obj/, .git/, etc.

### .gitignore (33 lines)
**Purpose**: Files to exclude from Git tracking
**Contains**: bin/, obj/, .vs/, .idea/, etc.

---

## 💻 Source Code - Home.Scout.Web

### Main Configuration

#### Program.cs (36 lines)
**Purpose**: ASP.NET Core entry point and DI configuration
**Key Points**:
- Service registration (DI container)
- HTTP pipeline configuration
- Razor Pages setup
- HttpClient setup for RSS service

#### Home.Scout.Web.csproj (16 lines)
**Purpose**: Project file with NuGet dependencies
**Dependencies**:
- Dapper 2.1.30 (ORM)
- Microsoft.Data.SqlClient 5.1.5 (Database)
- System.ServiceModel.Syndication 4.7.0 (RSS parsing)

#### appsettings.json (11 lines)
**Purpose**: Production configuration
**Contains**:
- Connection string (Docker database)
- Logging configuration
- Allowed hosts

#### appsettings.Development.json (11 lines)
**Purpose**: Development configuration
**Contains**:
- Connection string (LocalDB)
- Detailed error messages
- Logging levels

### Properties/

#### launchSettings.json
**Purpose**: VS launch profiles
**Contains**: Development server configuration

---

## 📦 Models (Domain Objects)

### Models/RssChannel.cs (14 lines)
**Purpose**: Represents an RSS feed channel
**Properties**:
- Id (int, PK)
- Name (string)
- Url (string)
- Description (string)
- CreatedAt (DateTime)
- UpdatedAt (DateTime)

### Models/NewsItem.cs (16 lines)
**Purpose**: Represents a news article
**Properties**:
- Id (int, PK)
- RssChannelId (int, FK)
- Title (string)
- Description (string)
- Link (string)
- PublishedAt (DateTime)
- CreatedAt (DateTime)

---

## 💾 Data Access Layer (Data folder)

### Interfaces

#### Data/IRssChannelRepository.cs (15 lines)
**Purpose**: Contract for channel data access
**Methods**:
- GetAllAsync()
- GetByIdAsync(id)
- AddAsync(channel)
- UpdateAsync(channel)
- DeleteAsync(id)

#### Data/INewsItemRepository.cs (18 lines)
**Purpose**: Contract for news item data access
**Methods**:
- GetAllAsync()
- GetByChannelIdAsync(channelId)
- GetByIdAsync(id)
- AddAsync(newsItem)
- UpdateAsync(newsItem)
- DeleteAsync(id)
- DeleteByChannelIdAsync(channelId)

### Implementations (Dapper)

#### Data/RssChannelRepository.cs (67 lines)
**Purpose**: MSSQL operations for channels using Dapper
**Implements**: IRssChannelRepository
**Operations**:
- Raw SQL with parameterized queries
- Connection pooling
- IDENTITY insert handling

#### Data/NewsItemRepository.cs (99 lines)
**Purpose**: MSSQL operations for news items using Dapper
**Implements**: INewsItemRepository
**Operations**:
- Raw SQL queries
- Batch operations
- Cascade delete support

### Database Scripts

#### Data/Scripts/001_Initial_Schema.sql (45 lines)
**Purpose**: Database initialization script
**Creates**:
- HomeScout database
- RssChannels table
- NewsItems table
- Indexes for performance

---

## 🔧 Services (Business Logic)

### Data/Services/IRssFeedService.cs (13 lines)
**Purpose**: Contract for RSS feed operations
**Methods**:
- FetchAndSaveNewsAsync(channel)
- FetchFeedAsync(feedUrl)

### Data/Services/RssFeedService.cs (115 lines)
**Purpose**: RSS feed parsing and synchronization
**Features**:
- SyndicationFeed parsing
- Duplicate detection
- Error handling and logging
- Description extraction
- HTML tag removal

---

## 🎨 Presentation Layer (Pages - Razor)

### Pages/Index.cshtml (32 lines)
**Purpose**: Home page landing
**Contains**: Navigation cards to features

### Pages/Index.cshtml.cs
**Purpose**: Home page code-behind

### Pages/Shared/

#### Pages/Shared/_Layout.cshtml (54 lines)
**Purpose**: Master layout template
**Contains**:
- Navigation bar
- Bootstrap CSS
- Bootstrap Icons CDN
- Footer
- RenderBody placeholder

#### Pages/Shared/_ValidationScriptsPartial.cshtml
**Purpose**: jQuery validation scripts

### Pages/_ViewImports.cshtml (5 lines)
**Purpose**: Global using statements for Razor
**Imports**:
- Models
- Data namespace
- Tag helpers

### Pages/_ViewStart.cshtml
**Purpose**: Layout configuration

### Channels Management (Pages/Channels/)

#### Pages/Channels/Index.cshtml (71 lines)
**Purpose**: List all RSS channels
**Features**:
- Table view
- Edit button
- Delete button
- Fetch button
- Add channel link
- Empty state message

#### Pages/Channels/Index.cshtml.cs (19 lines)
**Purpose**: Channels list code-behind
**Methods**: OnGetAsync()

#### Pages/Channels/Create.cshtml (41 lines)
**Purpose**: Add new RSS channel form
**Fields**:
- Channel name (required)
- Channel URL (required)
- Description (optional)
- Validation messages

#### Pages/Channels/Create.cshtml.cs (53 lines)
**Purpose**: Channel creation code-behind
**Methods**: OnPostAsync()
**Validation**: Fluent validation

#### Pages/Channels/Edit.cshtml (48 lines)
**Purpose**: Edit existing RSS channel form
**Features**:
- Pre-populated fields
- Same validation as Create

#### Pages/Channels/Edit.cshtml.cs (58 lines)
**Purpose**: Channel edit code-behind
**Methods**: OnGetAsync(id), OnPostAsync(id)

#### Pages/Channels/Delete.cshtml (34 lines)
**Purpose**: Delete confirmation page
**Features**:
- Shows channel details
- Confirmation message
- Delete button
- Cancel button

#### Pages/Channels/Delete.cshtml.cs (40 lines)
**Purpose**: Channel deletion code-behind
**Methods**: OnGetAsync(id), OnPostAsync(id)
**Features**: Cascade delete of news items

#### Pages/Channels/Fetch.cshtml (48 lines)
**Purpose**: Fetch RSS articles from channel
**Features**:
- Shows fetch results
- Lists fetched articles
- Success/error messages
- Navigation links

#### Pages/Channels/Fetch.cshtml.cs (50 lines)
**Purpose**: RSS fetch code-behind
**Methods**: OnGetAsync(id)
**Features**: Error handling and logging

### News Display (Pages/News/)

#### Pages/News/Index.cshtml (80 lines)
**Purpose**: Display all news articles
**Columns**:
- Date & time (sortable)
- Title (with link)
- Description
- Source (channel name)
- Delete button
**Features**:
- Table view
- Responsive design
- Link to manage channels
- Empty state

#### Pages/News/Index.cshtml.cs (27 lines)
**Purpose**: News list code-behind
**Methods**: OnGetAsync()
**Features**: Loads news + channel names mapping

#### Pages/News/Delete.cshtml (35 lines)
**Purpose**: Delete news article confirmation
**Features**:
- Shows article preview
- Confirmation dialog
- Delete/Cancel buttons

#### Pages/News/Delete.cshtml.cs (31 lines)
**Purpose**: News deletion code-behind
**Methods**: OnGetAsync(id), OnPostAsync(id)

---

## 📊 Summary Statistics

### File Counts by Type
- **Documentation**: 8 files
- **Configuration**: 4 files
- **Models**: 2 files
- **Data Access**: 4 files (2 interfaces, 2 implementations)
- **Services**: 2 files (1 interface, 1 implementation)
- **Database Scripts**: 1 file
- **Pages**: 14 files (Razor + code-behind)
- **Shared Components**: 3 files
- **Total .cs files**: ~25
- **Total .cshtml files**: ~15
- **Total lines of code**: ~1200+

### Code Organization
```
Home.Scout.Web/
├── Models/ (2 classes)
├── Data/ (6 files - interfaces, implementations, SQL)
├── Services/ (2 files - interface, implementation)
├── Pages/
│   ├── Channels/ (10 files - 5 pages with code-behind)
│   ├── News/ (4 files - 2 pages with code-behind)
│   └── Shared/ (3 templates)
├── wwwroot/ (Static files)
├── Properties/ (Launch settings)
└── Configuration files (appsettings.json, Program.cs, .csproj)
```

---

## 🔗 Key Dependencies

### NuGet Packages
- **Dapper** - ORM for data access
- **Microsoft.Data.SqlClient** - SQL Server connection
- **System.ServiceModel.Syndication** - RSS/Atom parsing

### Framework
- **ASP.NET Core 9.0** - Web framework
- **Razor Pages** - Page-based UI
- **Dependency Injection** - Built-in

### Frontend
- **Bootstrap 5** - CSS framework
- **Bootstrap Icons** - Icon library
- **jQuery** - JavaScript library
- **jQuery Validation** - Form validation

### External
- **Docker** - Containerization
- **SQL Server 2022** - Database
- **Git** - Version control

---

## 📈 File Growth Over Development

| Phase | Files | Lines | Purpose |
|-------|-------|-------|---------|
| Phase 1 | 2 | 200 | Initial setup |
| Phase 2 | 10 | 600 | Models + Data |
| Phase 3 | 20 | 1000 | Services + Pages |
| Phase 4 | 30 | 1300 | Documentation |

---

## 🔄 File Dependencies

```
Program.cs
    ├── Data/IRssChannelRepository.cs
    ├── Data/RssChannelRepository.cs
    ├── Data/INewsItemRepository.cs
    ├── Data/NewsItemRepository.cs
    ├── Services/IRssFeedService.cs
    └── Services/RssFeedService.cs

Pages/Channels/*
    ├── Data/IRssChannelRepository.cs
    ├── Data/INewsItemRepository.cs
    ├── Services/IRssFeedService.cs
    └── Models/*

Pages/News/*
    ├── Data/INewsItemRepository.cs
    ├── Data/IRssChannelRepository.cs
    └── Models/*

Services/RssFeedService.cs
    ├── Models/*
    ├── Data/INewsItemRepository.cs
    └── System.ServiceModel.Syndication
```

---

## 📝 Documentation Mapping

| File Type | Count | Documentation |
|-----------|-------|---|
| Configuration | 4 | README.md, DEVELOPMENT.md |
| Models | 2 | ARCHITECTURE.md |
| Repositories | 4 | ARCHITECTURE.md, DEVELOPMENT.md |
| Services | 2 | ARCHITECTURE.md, README.md |
| Pages | 14 | GETTING_STARTED.md, README.md |
| Database | 1 | DEVELOPMENT.md, ARCHITECTURE.md |

---

## 🎯 File Access Patterns

### Most Used
1. Program.cs - Application startup
2. RssFeedService.cs - Core business logic
3. Channels/Index.cshtml - Main interface
4. News/Index.cshtml - News display

### Database
1. RssChannelRepository.cs - Channel CRUD
2. NewsItemRepository.cs - Article CRUD
3. 001_Initial_Schema.sql - DB setup

### Configuration
1. appsettings.json - Production config
2. appsettings.Development.json - Dev config
3. docker-compose.yml - Container setup

---

## 🚀 Quick Reference

### To understand the project:
1. Start with **ARCHITECTURE.md** (diagrams)
2. Review **Program.cs** (setup)
3. Look at **Models** (data structure)
4. Explore **Pages** (UI)
5. Check **Services** (logic)

### To modify the project:
1. Update **Models** if schema changes
2. Update **Repositories** if queries change
3. Update **Services** for business logic
4. Update **Pages** for UI changes
5. Update **appsettings.json** for config

### To deploy the project:
1. Update **docker-compose.yml** for production
2. Update **Dockerfile** if needed
3. Update **appsettings.json** for production
4. Run **docker-compose up** in production

---

**Last Updated**: 2026-02-09  
**Version**: 1.0.0  
**Status**: Complete ✅

