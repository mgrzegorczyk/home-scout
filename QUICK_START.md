# 🚀 Quick Start - Home Scout

## ⚡ Uruchomienie w 2 minuty (Docker)

```bash
docker-compose up --build
```

Otwórz: **http://localhost:8080**

---

## 🔧 Lokalny development (.NET 9.0)

```bash
# Tworzenie bazy danych
sqlcmd -S (localdb)\mssqllocaldb -i "src\Home.Scout.Web\Data\Scripts\001_Initial_Schema.sql"

# Uruchomienie aplikacji
cd src\Home.Scout.Web
dotnet run
```

Otwórz: **https://localhost:5001**

---

## 📋 Pierwsze kroki

1. **Dodaj kanał RSS** → Menu "Kanały" → "Dodaj kanał"
2. **Pobierz artykuły** → Kliknij "Pobierz" przy kanale
3. **Przeglądaj newsy** → Menu "Newsy"

---

## 📚 Więcej informacji

- **INSTALLATION.md** - Instrukcje instalacji
- **DEVELOPMENT.md** - Development guide
- **README.md** - Pełna dokumentacja

---

## 🐛 Problemy?

```bash
# Czyszczenie
docker-compose down -v
docker system prune -a

# Przebudowa
docker-compose up --build
```

---

## ✅ Gotowe! Zapraszamy do użytkowania! 🎉

