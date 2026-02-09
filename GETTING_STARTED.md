# 🎯 Getting Started - Home Scout

Witaj w **Home Scout** - aplikacji do agregacji newsów z kanałów RSS!

---

## ⚡ Super szybki start (2 minuty)

### 1️⃣ Masz Docker?
```bash
docker-compose up --build
```
Otwórz: **http://localhost:8080**

### 2️⃣ Nie masz Dockera? Masz .NET?
```bash
# Tworzenie bazy danych (Windows)
sqlcmd -S (localdb)\mssqllocaldb -i "src\Home.Scout.Web\Data\Scripts\001_Initial_Schema.sql"

# Uruchomienie
cd src\Home.Scout.Web && dotnet run
```
Otwórz: **https://localhost:5001**

---

## 📚 Gdzie znaleźć informacje?

| Plik | Treść |
|------|-------|
| **QUICK_START.md** | ⚡ 2-minutowy start |
| **INSTALLATION.md** | 📋 Szczegółowe instrukcje instalacji |
| **README.md** | 📖 Pełna dokumentacja |
| **DEVELOPMENT.md** | 🔧 Dla developerów |
| **ARCHITECTURE.md** | 🏗️ Opis architektury |
| **SUMMARY.md** | ✅ Co zostało zrobione |
| **RSS_FEEDS_SAMPLES.md** | 📡 Przykładowe kanały RSS |

---

## 🎬 Pierwsze kroki w aplikacji

### Krok 1: Dodaj kanał RSS
1. Kliknij "📡 Kanały" w górnym menu
2. Kliknij "Dodaj kanał"
3. Wypełnij formularz:
   ```
   Nazwa: BBC News
   URL: https://feeds.bbc.co.uk/news/rss.xml
   Opis: (opcjonalnie)
   ```
4. Kliknij "Dodaj kanał"

### Krok 2: Pobierz artykuły
1. Na stronie kanałów zobaczysz nowy kanał
2. Kliknij przycisk "Pobierz" (niebieska ikona ze strzałką)
3. Czekaj na załadowanie artykułów
4. Zobaczysz liczbę pobranych newsów

### Krok 3: Przeglądaj newsy
1. Kliknij "📰 Newsy" w górnym menu
2. Zobaczysz tabelę ze wszystkimi artykułami:
   - Data i godzina publikacji
   - Tytuł (linkuje do źródła)
   - Krótki opis
   - Źródło (nazwa kanału)
3. Kliknij na tytuł aby przejść do artykułu

### Krok 4: Zarządzaj kanałami
- **Edytuj**: Kliknij "Edytuj" obok kanału
- **Usuń**: Kliknij "Usuń" (artykuły też się usunę)
- **Pobierz ponownie**: Kliknij "Pobierz" aby aktualizować artykuły

---

## 🌐 Rekomendowane kanały do testowania

### Start z tym
```
BBC News
https://feeds.bbc.co.uk/news/rss.xml
```

### Dodaj więcej
```
The Guardian
https://www.theguardian.com/world/rss

Hacker News
https://news.ycombinator.com/rss

TechCrunch
https://techcrunch.com/feed/
```

Więcej kanałów: Przejrzyj **RSS_FEEDS_SAMPLES.md**

---

## 🆘 Szybka pomoc

### "Nie mogę się połączyć z bazą"
```bash
# Upewnij się że SQL Server jest uruchomiony
# Jeśli używasz LocalDB:
sqllocaldb start mssqllocaldb

# Stwórz bazę:
sqlcmd -S (localdb)\mssqllocaldb -i "src\Home.Scout.Web\Data\Scripts\001_Initial_Schema.sql"
```

### "Docker się nie uruchamia"
```bash
# Sprawdź czy Docker Desktop jest włączony
# Sprawdź porty:
netstat -ano | findstr :8080

# Jeśli zajęty, edytuj docker-compose.yml
```

### "RSS feed'y się nie pobierają"
1. Sprawdź czy URL jest prawidłowy
2. Sprawdź czy RSS feed jest dostępny (otwórz w przeglądarce)
3. Sprawdź logi: `docker-compose logs web`

### "Nie widzę artykułów"
1. Dodaj kanał RSS
2. Kliknij "Pobierz"
3. Idź do "Newsy"
4. Odśwież stronę (F5)

Jeśli nic nie pomogło: Przejrzyj **DEVELOPMENT.md** sekcja "Troubleshooting"

---

## 🎯 Co możesz robić?

✅ Dodawać kanały RSS  
✅ Edytować kanały  
✅ Usuwać kanały  
✅ Pobierać artykuły  
✅ Przeglądać newsy  
✅ Usuwać artykuły  
✅ Klikać na artykuły aby przejść do źródła  

---

## 🔄 Workflow

```
1. Dodaj kanał (Kanały → Dodaj kanał)
   ↓
2. Pobierz artykuły (Kanały → Pobierz)
   ↓
3. Przeglądaj newsy (Newsy)
   ↓
4. Edytuj/Usuń (opcjonalnie)
```

---

## 💡 Porady

1. **Zacznij od BBC News** - zawsze ma artykuły
2. **Używaj różnych kanałów** - aby mieć różne źródła
3. **Pobieraj co kilka godzin** - aby być na bieżąco
4. **Czyszczenie** - usuwaj stare artykuły które cię nie interesują

---

## 📞 Potrzebujesz więcej pomocy?

1. **INSTALLATION.md** - Jeśli masz problemy z instalacją
2. **DEVELOPMENT.md** - Jeśli chcesz modyfikować kod
3. **README.md** - Jeśli chcesz wiedzieć wszystko
4. **ARCHITECTURE.md** - Jeśli chcesz zrozumieć architekturę

---

## ✨ Готово!

Możesz teraz zacząć używać Home Scout! 🎉

1. Uruchom aplikację
2. Dodaj kanał RSS
3. Pobierz artykuły
4. Ciesz się newsami!

Powodzenia! 🚀

