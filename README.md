# Examples WPF

This solution contains .NET 10 examples for Windows, including WPF applications and a local ASP.NET Core API:

## CustomWpfControl

Demonstrates a reusable templated WPF control. `CustomTextControl` exposes a two-way-bindable `Text` dependency property and uses a style in `Themes/Generic.xaml` to render an editable `TextBox`. The template also uses an attached behavior to select all text when the input receives focus.

## DependencyInjectionWpf

Demonstrates dependency injection in WPF with `Microsoft.Extensions.Hosting`. The generic host registers an application service, viewmodel, and window; the window receives its viewmodel through constructor injection, and the viewmodel receives `IGreetingService`. Run it with `dotnet run --project DependencyInjectionWpf/DependencyInjectionWpf.csproj`.

## ErrorHandlingLoggingWpf

Demonstrates error handling and logging in a WPF application. Serilog writes daily rolling logs to `%LOCALAPPDATA%/ExamplesWpf/ErrorHandlingLoggingWpf/logs` and to Visual Studio's Debug output. The UI includes a successful operation, a handled operation failure, and an unhandled dispatcher failure; the last one is logged and shown to the user but is not suppressed. Run it with `dotnet run --project ErrorHandlingLoggingWpf/ErrorHandlingLoggingWpf.csproj`.

## SecurePasswordInput

Demonstrates binding a WPF `PasswordBox` to a viewmodel `SecureString` through an attached behavior, since `PasswordBox.Password` is not a bindable dependency property. The sample also converts the secure value to a regular `string` for display as `PlainPassword`; that conversion exposes the password in managed memory and is included for demonstration only, not as a recommended production practice.

## Authentication and authorization

`AuthApi` is an ASP.NET Core API backed by SQLite and ASP.NET Core Identity. It issues 30-minute JWT bearer tokens and seeds two roles with permission claims: `Reader` can read reports (`reports.read`), while `Administrator` can also manage users (`users.manage`). The API demonstrates authenticated identity, permission-protected reports, and an administrator-only user list.

`AuthWpfClient` is a WPF desktop client that signs in to the API over HTTPS, keeps the JWT in memory, and calls the protected endpoints. Signing out clears the token. A Reader can view reports but receives `403 Forbidden` for user management; an Administrator can access both.

Demo accounts:

| Username | Password | Role |
| --- | --- | --- |
| `reader@demo.local` | `Reader123!` | Reader |
| `admin@demo.local` | `Admin123!` | Administrator |

To run the authentication sample, trust the local HTTPS development certificate (`dotnet dev-certs https --trust`), then start the API with `dotnet run --project AuthApi/AuthApi.csproj`. Start `AuthWpfClient` from Visual Studio or with `dotnet run --project AuthWpfClient/AuthWpfClient.csproj`. The API creates `auth-demo.db` and seeds the demo accounts on its first run.

The seeded passwords and JWT signing key in `AuthApi/appsettings.json` are for local demonstration only. Do not use them or the sample signing key in a deployed application; use secure secret management and an appropriate identity deployment instead.

## gRPC client and backend

`GrpcBackend` is an ASP.NET Core gRPC service exposing a unary `SayHello` RPC defined in `GrpcBackend/Protos/greeting.proto`. `GrpcWpfClient` compiles the same protobuf contract and calls the service over HTTPS/HTTP2.

Trust the local HTTPS development certificate (`dotnet dev-certs https --trust`), start the backend with `dotnet run --project GrpcBackend/GrpcBackend.csproj`, then run the client with `dotnet run --project GrpcWpfClient/GrpcWpfClient.csproj`. The backend listens on `https://localhost:7044`.

An additional Python implementation is available in `GrpcPythonBackend`. From the repository root, trust the local development certificate (`dotnet dev-certs https --trust`) and run `GrpcPythonBackend/run.cmd`. The script creates an isolated Python virtual environment, installs the gRPC packages, generates Python stubs from the shared proto, exports the dev certificate outside the repository, and starts the server at `https://localhost:7045`. In the WPF client's backend address field, use `https://localhost:7045` for Python or `https://localhost:7044` for C#.

## Run the examples

Open `Examples_WPF.slnx` in Visual Studio, choose either project as the startup project, and run it on Windows with the .NET 10 SDK installed.
