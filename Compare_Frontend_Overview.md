# React vs. Angular vs. Blazor – Überblick

Alle drei Beispielprojekte setzen denselben Ticket Tracker um (gleiche Funktionen, gleiches Aussehen):

- `Compare_React_TicketTracker` – React 18 + TypeScript + Vite
- `Compare_Angular_TicketTracker` – Angular 17 + TypeScript
- `Compare_Blazor_TicketTracker` – Blazor WebAssembly (.NET 10) + C#

Streng genommen ist React eine **Bibliothek**, Angular und Blazor sind **Frameworks**.

## Grundphilosophie

| | React | Angular | Blazor (WebAssembly) |
|---|---|---|---|
| Typ | UI-Bibliothek (nur View-Schicht) | Komplettes Framework | Komplettes Framework |
| Entwickelt von | Meta | Google | Microsoft |
| Sprache | JavaScript/TypeScript (JSX/TSX) | TypeScript | **C#** (Razor-Syntax) |
| Läuft im Browser als | JavaScript | JavaScript | **WebAssembly** (.NET-Runtime im Browser) |
| Routing, Forms, HTTP | Zusatzbibliotheken | Eingebaut | Eingebaut (`@page`, `EditForm`, `HttpClient`) |
| Vorgaben | Viel Freiheit | Strikte Struktur | Mittlere Struktur (.NET-Konventionen) |
| Ökosystem | npm | npm | NuGet (und JS über Interop) |
| Build-Tool | Vite (frei wählbar) | Angular CLI | `dotnet` CLI / MSBuild |
| Erststart | schnell | schnell | langsamer (.NET-Runtime wird geladen) |
| Code teilen mit Backend | nur Typen/Verträge | nur Typen/Verträge | **direkt** (gleiche C#-Klassen im Backend) |
| Einstieg für | Web-Entwickler | Enterprise-Web-Entwickler | .NET/C#-Entwickler |

## Konzepte im Code

| Thema | React | Angular | Blazor |
|---|---|---|---|
| Komponente | Funktion, die JSX zurückgibt | Klasse mit `@Component` | `.razor`-Datei (Markup + `@code`-Block) |
| Template | JSX: `{items.map(...)}` | HTML-Datei: `@for`, `@if`, `{{ }}` | Razor: `@foreach`, `@if`, `@Wert` |
| Zustand | `useState`, immutable | Klassen-Felder oder `signal()` | Felder/Properties; Re-Render nach Events automatisch |
| Abgeleitete Werte | `useMemo` | `computed()` | normale Property (wird bei jedem Rendern neu berechnet) |
| Initialisierung/Seiteneffekte | `useEffect` | `effect()`, `ngOnInit` | `OnInitializedAsync`, `OnParametersSet` |
| Daten an Kinder | Props | `@Input()` | `[Parameter]` |
| Ereignisse an Eltern | Callback-Props | `@Output()` + `EventEmitter` | `EventCallback<T>` |
| Formularfelder | `value` + `onChange` manuell | `[(ngModel)]` | `@bind` / `@bind:event="oninput"` |
| Geteilter Zustand | Hooks, Context, Redux | Service + `inject()` | Service + `@inject` (in `Program.cs` registriert) |
| Dependency Injection | nicht eingebaut | eingebaut | eingebaut (`AddScoped`, `AddSingleton`) |
| Bedingte CSS-Klassen | Template-String | `[class]`, `[ngClass]` | `class="ticket @status.Css()"` |
| Listen-Identität | `key={id}` | `track t.id` | `@key="t.Id"` |
| Datumsformat | `toLocaleDateString()` | `DatePipe` | `DateTime.ToString("dd.MM.yyyy")` |
| Typen | TS-Types/Interfaces | TS-Interfaces | `record`, `enum`, Pattern Matching |
| Zugriff auf Browser-APIs | direkt (`localStorage`) | direkt (`localStorage`) | **nur über JS-Interop** (`IJSRuntime`) |
| Unveränderliche Kopie | `{ ...t, status }` | `{ ...t, status }` | `t with { Status = status }` |
| HTTP-Aufrufe | `fetch` (hier in `api.ts`) | `HttpClient` + Observables + `subscribe()` | `HttpClient` + `async/await` + `GetFromJsonAsync` |
| Laden beim Start | `useEffect(..., [])` | Konstruktor des Services | `OnInitializedAsync` |
| JSON/Enums | Strings wie `"in-progress"` direkt | Strings direkt | `JsonStringEnumConverter` nötig, um C#-Enums auf Strings abzubilden |
| Fehlerbehandlung | `try/catch` bzw. `.catch()` | `subscribe({ error })` | `try/catch` (`ApiException`) |
| Konfiguration (API-URL) | `.env` mit `VITE_API_URL`, Zugriff per `import.meta.env` | `environments/environment.ts` (per `fileReplacements` je Build getauscht) | `wwwroot/appsettings.json`, Zugriff per `builder.Configuration` |
| Routing | React Router (`<Routes>`, `<Route>`) | `provideRouter(routes)`, `<router-outlet>` | `@page "/pfad"`, `<Router>` in `App.razor` |
| Route-Parameter | `useParams()` | `@Input() id` (mit `withComponentInputBinding()`) | `{Id:int}` im `@page` + `[Parameter]` |
| Navigation | `<Link>`, `useNavigate()` | `routerLink`, `Router.navigate()` | `<a href>`, `NavigationManager.NavigateTo()` |
| Route Guard | `<RequireAuth>`-Komponente mit `<Navigate>` | `CanActivateFn` (`authGuard`) | `<RequireAuth>`-Komponente mit `NavigateTo()` |
| Login-Zustand | `AuthContext` + `sessionStorage` | `AuthService` mit `signal()` + `sessionStorage` | `AuthService` (Singleton) + `sessionStorage` per JS-Interop |
| Token an Requests anhängen | zentrale `request()`-Funktion in `api.ts` | `HttpInterceptorFn` (`auth.interceptor.ts`) | `DelegatingHandler` (`AuthHeaderHandler`) |
| Formularvalidierung | von Hand: Fehlertext aus dem State abgeleitet | Reactive Forms: `FormBuilder` + `Validators` | `EditForm` + `DataAnnotationsValidator` + Attribute (`[Required]`, `[StringLength]`) |
| Fehlermeldung am Feld | `{error && <span>}` | `@if (control.invalid && touched)` | `<ValidationMessage For="...">` |
| Lade-/Fehlerzustand | `loading`/`error`-State, `.finally()` | `loading`/`loadError`-Signals | `Loading`/`LoadError`-Properties |
| Komponententests | Vitest + Testing Library (`render`, `screen`, `userEvent`) | Jasmine/Karma + `TestBed` + `HttpTestingController` | bUnit (`Render<T>()`, `Find()`, `Change()`, `Submit()`) |

## Datenfluss und Änderungserkennung

- **React**: Einseitiger Datenfluss. Ändert sich der State, wird die Funktion **neu ausgeführt** und ein Virtual DOM verglichen. Deshalb ist Immutability wichtig.
- **Angular**: **Change Detection** (mit Signals feingranular) aktualisiert nur betroffene DOM-Stellen. Direktes Verändern ist möglich.
- **Blazor**: Nach Event-Handlern rendert die Komponente neu und es wird ein **Render-Tree-Diff** auf das DOM angewendet. Bei Änderungen außerhalb von Events (Timer, Service-Events) muss `StateHasChanged()` aufgerufen werden.

## Dependency Injection

- **Angular** und **Blazor**: fester Bestandteil. Services werden zentral registriert (`providedIn: 'root'` bzw. `AddScoped`) und per `inject()` / `@inject` geholt.
- **React**: kein eigenes DI-System. Stattdessen `Context`, Hooks oder Props.

## Projektstruktur im Beispiel

| React | Angular | Blazor |
|---|---|---|
| `main.tsx` (Router + `AuthProvider`) | `main.ts` + `app.config.ts` (Router, HttpClient, Interceptor) | `Program.cs` (HttpClient, Handler, Services) |
| `App.tsx` (Layout + Routen) | `app.component.*` + `app.routes.ts` | `Layout/MainLayout.razor` + Router in `App.razor` |
| `TicketsPage.tsx`, `TicketDetailPage.tsx`, `LoginPage.tsx` | `tickets-page.component.*`, `ticket-detail.component.*`, `login.component.*` | `Pages/Home.razor`, `Pages/TicketDetail.razor`, `Pages/Login.razor` |
| `TicketForm.tsx`, `TicketItem.tsx` | `ticket-form.component.*`, `ticket-item.component.*` | `Components/TicketForm.razor`, `Components/TicketItem.razor` |
| `api.ts`, `auth.tsx`, `tokenStore.ts` | `ticket.service.ts`, `auth.service.ts`, `auth.interceptor.ts`, `auth.guard.ts` | `Services/TicketService.cs`, `Services/AuthService.cs`, `Components/RequireAuth.razor` |
| `types.ts` | `ticket.model.ts` | `Models/Ticket.cs` |
| `App.css` | `styles.css` | `wwwroot/css/app.css` |
| `.env`, `vite.config.ts`, `package.json` | `environments/*`, `angular.json`, `package.json` | `wwwroot/appsettings.json`, `*.csproj` |
| `*.test.tsx` (Vitest) | `*.spec.ts` (Jasmine) | `Compare_Blazor_TicketTracker.Tests` (bUnit, xUnit) |

## Backend (`Compare_TicketApi`)

Alle drei Frontends sprechen dieselbe ASP.NET-Core-Minimal-API an (`http://localhost:5300`). Die Tickets liegen in einer SQLite-Datenbank (`tickets.db`, EF Core, wird beim ersten Start per Migration angelegt und mit drei Beispiel-Tickets befüllt).

| Methode | Pfad | Zweck |
|---|---|---|
| POST | `/api/auth/login` | Anmelden, liefert ein JWT (Demo-Zugang `demo` / `demo123`) |
| GET | `/api/tickets` | Alle Tickets laden |
| GET | `/api/tickets/{id}` | Einzelnes Ticket |
| POST | `/api/tickets` | Ticket anlegen (`title`, `description`, `priority`) |
| PUT | `/api/tickets/{id}` | Titel, Beschreibung, Priorität ändern |
| PUT | `/api/tickets/{id}/status` | Status ändern (`status`) |
| DELETE | `/api/tickets/{id}` | Ticket löschen |

Alle `/api/tickets`-Endpunkte erfordern ein gültiges Token (`Authorization: Bearer ...`), sonst antwortet die API mit 401.

**Was das Backend zusätzlich zeigt**

- **Konfiguration:** Port, CORS-Ursprünge, Connection-String und JWT-Einstellungen stehen in `appsettings.json`. Der JWT-Schlüssel steht nur in `appsettings.Development.json` (nur für die Entwicklung; in echten Umgebungen per User-Secrets oder Umgebungsvariable `Jwt__Key`).
- **Migrationen:** `Migrations/` (EF Core); neue Änderungen mit `dotnet ef migrations add <Name> --project Compare_TicketApi`.
- **DTOs:** Die API gibt `TicketDto` zurück, nicht die Datenbank-Entität `Ticket`.
- **Validierung:** DataAnnotations auf den Request-Typen (`AddValidation()`), Fehler als `ProblemDetails` mit Status 400.
- **OpenAPI:** Beschreibung unter `/openapi/v1.json`, Oberfläche (Scalar) unter `/scalar` (nur in Development).
- **Tests:** `Compare_TicketApi.Tests` (xUnit, `WebApplicationFactory`, eigene SQLite-Datei pro Testlauf).

Suche und Filter laufen weiterhin im Frontend. Die Frontends unterscheiden sich darin, **wie** sie die API aufrufen, das Token anhängen und das Ergebnis in ihren Zustand übernehmen (siehe Tabelle oben). Beim Blazor-Client sind die Modelle in C# nachgebaut; da Frontend und Backend beide .NET sind, könnten sie sich in einem gemeinsamen Projekt dieselben Klassen teilen. Bei React/Angular müssen die TypeScript-Typen manuell synchron gehalten werden.

**Bewusste Vereinfachungen** (nicht für Produktion): Der Demo-Benutzer steht in der Konfiguration (echte Projekte: Benutzer-Datenbank und Passwort-Hashing, z. B. ASP.NET Identity). Das Token liegt in `sessionStorage` und ist damit für Skripte auf der Seite lesbar (XSS-Risiko; Alternative: HttpOnly-Cookie).

## Wann was?

- **React**: Kleine bis mittlere Apps, maximale Flexibilität, riesiges Ökosystem (auch Basis für Next.js).
- **Angular**: Große Unternehmensanwendungen mit vielen Entwicklern und einheitlichen Konventionen.
- **Blazor**: Teams, die bereits in C#/.NET zuhause sind und Modelle, Validierung und Logik mit dem Backend teilen wollen; weniger JavaScript-Wissen nötig. Nachteile: größerer Download beim Start, kleinere Community für UI-Bibliotheken.

## Starten

```powershell
# Backend (zuerst starten)
cd Compare_TicketApi; dotnet run                              # http://localhost:5300

# React
cd Compare_React_TicketTracker; npm install; npm run dev     # http://localhost:5173

# Angular
cd Compare_Angular_TicketTracker; npm install; npx ng serve  # http://localhost:4200

# Blazor
cd Compare_Blazor_TicketTracker; dotnet run --urls http://localhost:5200
```

Anmeldung in allen Frontends: **demo / demo123**.

**Tests**

```powershell
dotnet test Compare_TicketApi.Tests
dotnet test Compare_Blazor_TicketTracker.Tests
cd Compare_React_TicketTracker; npm test
cd Compare_Angular_TicketTracker; npx ng test --watch=false --browsers=ChromeHeadless
```

Die Angular-Tests brauchen einen Chromium-Browser. Ist kein Chrome installiert, kann Edge genutzt werden: `$env:CHROME_BIN = "C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"` vor dem Aufruf setzen.

**VS Code:** `Terminal > Run Task` bietet `Start all (API + React + Angular + Blazor)` und `Test all`; unter `Run and Debug` gibt es Compound-Konfigurationen wie `Debug: API + React` (Frontend-Server vorher per Task starten).
