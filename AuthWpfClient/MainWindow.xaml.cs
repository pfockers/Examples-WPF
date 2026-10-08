using System.Windows;

namespace AuthWpfClient;

public partial class MainWindow : Window
{
    private readonly AuthApiClient _apiClient = new();

    public MainWindow()
    {
        InitializeComponent();
        Closed += (_, _) => _apiClient.Dispose();
    }

    private async void OnSignInClick(object sender, RoutedEventArgs e)
    {
        OutputTextBox.Text = await _apiClient.SignInAsync(UserNameTextBox.Text, PasswordInput.Password);
        PasswordInput.Clear();
    }

    private async void OnCurrentUserClick(object sender, RoutedEventArgs e) =>
        OutputTextBox.Text = await _apiClient.GetAsync("/api/auth/me");

    private async void OnReadReportsClick(object sender, RoutedEventArgs e) =>
        OutputTextBox.Text = await _apiClient.GetAsync("/api/reports");

    private async void OnManageUsersClick(object sender, RoutedEventArgs e) =>
        OutputTextBox.Text = await _apiClient.GetAsync("/api/admin/users");

    private void OnSignOutClick(object sender, RoutedEventArgs e)
    {
        _apiClient.SignOut();
        OutputTextBox.Text = "Signed out. The access token was cleared from memory.";
    }
}
