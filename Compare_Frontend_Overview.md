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
| Fehlerbehandlung | `try/catch` bzw. `.catch()` | `subscribe({ error })` | `try/catch` (`HttpRequestException`) |

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
| `main.tsx` | `main.ts` + `app.config.ts` | `Program.cs` |
| `App.tsx` (State + Logik + View) | `ticket.service.ts` + `app.component.*` | `Services/TicketService.cs` + `Pages/Home.razor` |
| `TicketForm.tsx`, `TicketItem.tsx` | `ticket-form.component.*`, `ticket-item.component.*` | `Components/TicketForm.razor`, `Components/TicketItem.razor` |
| `types.ts` | `ticket.model.ts` | `Models/Ticket.cs` |
| `App.css` | `styles.css` | `wwwroot/css/app.css` |
| `package.json`, `vite.config.ts` | `package.json`, `angular.json` | `*.csproj` |
| `index.html` | `index.html` | `wwwroot/index.html` |

## Backend (`Compare_TicketApi`)

Alle drei Frontends sprechen dieselbe ASP.NET-Core-Minimal-API an (`http://localhost:5300`). Die Tickets liegen in einer SQLite-Datenbank (`tickets.db`, EF Core, wird beim ersten Start angelegt und mit drei Beispiel-Tickets befüllt).

| Methode | Pfad | Zweck |
|---|---|---|
| GET | `/api/tickets` | Alle Tickets laden |
| POST | `/api/tickets` | Ticket anlegen (`title`, `priority`) |
| PUT | `/api/tickets/{id}/status` | Status ändern (`status`) |
| DELETE | `/api/tickets/{id}` | Ticket löschen |

Suche und Filter laufen weiterhin im Frontend. Die Frontends unterscheiden sich nur darin, **wie** sie die API aufrufen und das Ergebnis in ihren Zustand übernehmen (siehe Tabelle oben). Beim Blazor-Client sind die Modelle in C# nachgebaut; da Frontend und Backend beide .NET sind, könnten sie sich in einem gemeinsamen Projekt dieselben Klassen teilen. Bei React/Angular müssen die TypeScript-Typen manuell synchron gehalten werden.

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
