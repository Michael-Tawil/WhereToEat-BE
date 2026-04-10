# WhereToEat API 🍜

The backend API for WhereToEat — an AI-powered restaurant discovery app. Built with C# ASP.NET Core, orchestrating Google Places and Gemini AI to deliver personalised restaurant suggestions.

**Live API:** [wheretoeat-be.onrender.com](https://wheretoeat-be.onrender.com)  
**Frontend:** [wheretoeatcom.netlify.app](https://wheretoeatcom.netlify.app)

---

## Features

- **AI Suggestion** — Calls Google Places API for real restaurant data, passes results to Gemini AI for intelligent recommendation
- **JWT Authentication** — Secure register and login with hashed passwords
- **Favourites** — Full CRUD for saving and removing favourite restaurants
- **Last Visited** — Track visited restaurants, returns most recent 20
- **Protected Endpoints** — All user data endpoints require valid JWT token

---

## Tech Stack

- **Framework:** C# ASP.NET Core (.NET 10)
- **Database:** Supabase (PostgreSQL) via Entity Framework Core
- **AI:** Google Gemini API
- **Restaurant Data:** Google Places API (New)
- **Auth:** JWT Bearer tokens + BCrypt password hashing
- **HTTP Client:** RestSharp
- **Deployed:** Render + Docker

---

## Architecture

```
POST /api/aisuggest/suggest
        ↓
PlacesService → Google Places API
Returns 20 real restaurants
        ↓
AIService → Gemini API
AI picks best match, returns JSON
        ↓
SuggestionResponse → Frontend
```

---

## Endpoints

### Auth
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | Login, returns JWT token |

### AI Suggestion (Protected)
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/aisuggest/suggest` | Get AI restaurant suggestion |

### Favourites (Protected)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/favourites` | Get user's favourites |
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
- Supabase account
- Google Places API key
- Google Gemini API key

### Installation

```bash
git clone https://github.com/Michael-Tawil/WhereToEat-BE.git
cd WhereToEat-BE
dotnet restore
```

### Environment Variables

Create an `appsettings.json` file:

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
    "Secret": "your-gemini-api-key"
  }
}
```

### Run locally

```bash
dotnet run
```

API runs on `https://localhost:7134` — Swagger UI available at `/swagger/index.html`

---

## Roadmap (V2)

- Smart prompt using user's favourites and visit history
- Rotation logic — exclude recently visited places
- Caching Google Places results to reduce API calls
- Rate limiting per user
- Retry logic for AI failures
- Background jobs for data maintenance

---

## Author

**Michael Tawil**  
