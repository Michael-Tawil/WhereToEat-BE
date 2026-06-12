# WhereToEat API 🍜

The backend API for WhereToEat - an AI-powered restaurant discovery app. Built with C# ASP.NET Core, orchestrating Google Places and OpenAI to deliver personalised restaurant suggestions.

**Live API:** [wheretoeat-be.onrender.com](https://wheretoeat-be.onrender.com)
**Frontend:** [wheretoeatcom.netlify.app](https://wheretoeatcom.netlify.app)
**Frontend repo:** [WhereToEat](https://github.com/Michael-Tawil/WhereToEat)

---

## Features

- **AI Suggestion** - Calls Google Places API for real restaurant data, passes results to an LLM (OpenAI gpt-4o-mini) for an intelligent recommendation
- **Smart history awareness** - excludes restaurants the user has already been suggested or visited, and uses favourites to infer taste
- **Dynamic pagination** - fetches additional pages of Google Places results until enough unseen restaurants are available
- **JWT Authentication** - register and login with hashed passwords (BCrypt)
- **Favourites** - full CRUD for saving and removing favourite restaurants
- **Last Visited** - track visited restaurants, returns most recent 20
- **Protected Endpoints** - all user data endpoints require a valid JWT token

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | C# ASP.NET Core (.NET 10) |
| Database | Supabase (PostgreSQL) via Entity Framework Core |
| AI | OpenAI API (gpt-4o-mini) |
| Restaurant Data | Google Places API (New) |
| Auth | JWT Bearer tokens + BCrypt password hashing |
| HTTP Client | RestSharp (Places), OpenAI SDK (AI) |
| Deployment | Docker → Render |

---

## Architecture

```
POST /api/aisuggest/suggest
        ↓
PlacesService → Google Places API
Returns up to 20 real restaurants per page (paginated as needed)
        ↓
AIService
  - fetches user's favourites, last visited, and suggestion history from DB
  - filters out restaurants already suggested or visited
  - builds a prompt with the filtered pool + user taste signals
        ↓
OpenAI (gpt-4o-mini) → picks best match, returns JSON
        ↓
SuggestionResponse → Frontend
  - suggestion is saved to suggestion history
  - matched against the Places pool to attach a Google Maps link
```

---

## Endpoints

### Auth
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Login, returns JWT token |

### AI Suggestion (Protected)
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/aisuggest/suggest` | Get an AI restaurant suggestion |

### Favourites (Protected)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/favourites` | Get the user's favourites |
| POST | `/api/favourites` | Add a favourite |
| DELETE | `/api/favourites/{id}` | Remove a favourite |

### Last Visited (Protected)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/lastvisited` | Get last 20 visited restaurants |
| POST | `/api/lastvisited` | Add a visited restaurant |

---

## Getting Started

### Prerequisites
- .NET 10 SDK
- Supabase account (PostgreSQL connection string)
- Google Places API key
- OpenAI API key (with billing enabled)

### Installation

```bash
git clone https://github.com/Michael-Tawil/WhereToEat-BE.git
cd WhereToEat-BE
dotnet restore
```

### Environment Variables

Create an `appsettings.json` (or set via environment variables on Render):

```json
{
  "Jwt": {
    "Secret": "your-jwt-secret"
  },
  "DB": {
    "Secret": "your-supabase-connection-string"
  },
  "Places": {
    "Secret": "your-google-places-api-key"
  },
  "AI": {
    "Secret": "your-openai-api-key"
  }
}
```

On Render, these are set as `Jwt__Secret`, `DB__Secret`, `Places__Secret`, and `AI__Secret` (double underscore maps to `:` in .NET configuration).

### Run locally

```bash
dotnet run
```

API runs on `https://localhost:7134` - Swagger UI available at `/swagger/index.html`

---

## Database Tables (Supabase)

- `users` - id, email, password_hash
- `favourites` - id, user_id, restaurant_name, address, rating, cuisine, price_range, created_at
- `last_visited` - id, user_id, restaurant_name, address, rating, cuisine, price_range, visited_at
- `suggested` - id, user_id, restaurant_name, suggested_at

---

## Notes on AI Provider

This project originally used Google's Gemini API, but Gemini's API enforces geographic restrictions that blocked requests from Render's hosting region. The AI layer was migrated to OpenAI (gpt-4o-mini), which has no such restriction and offers comparable quality for this use case at low cost.

---

## Author

**Michael Tawil**
