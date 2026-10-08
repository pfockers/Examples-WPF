# Examples WPF

This solution contains two .NET 10 WPF examples for Windows:

## CustomWpfControl

Demonstrates a reusable templated WPF control. `CustomTextControl` exposes a two-way-bindable `Text` dependency property and uses a style in `Themes/Generic.xaml` to render an editable `TextBox`. The template also uses an attached behavior to select all text when the input receives focus.

## SecurePasswordInput

Demonstrates binding a WPF `PasswordBox` to a viewmodel `SecureString` through an attached behavior, since `PasswordBox.Password` is not a bindable dependency property. The sample also converts the secure value to a regular `string` for display as `PlainPassword`; that conversion exposes the password in managed memory and is included for demonstration only, not as a recommended production practice.

## Run the examples

Open `Examples_WPF.slnx` in Visual Studio, choose either project as the startup project, and run it on Windows with the .NET 10 SDK installed.
