# Examples WPF

This solution contains .NET 10 examples for Windows, including WPF applications and a local ASP.NET Core API:

## CustomWpfControl

Demonstrates a reusable templated WPF control. `CustomTextControl` exposes a two-way-bindable `Text` dependency property and uses a style in `Themes/Generic.xaml` to render an editable `TextBox`. The template also uses an attached behavior to select all text when the input receives focus.

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

## Run the examples

Open `Examples_WPF.slnx` in Visual Studio, choose either project as the startup project, and run it on Windows with the .NET 10 SDK installed.
