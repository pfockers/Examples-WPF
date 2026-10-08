using System.Windows;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcExample;

namespace GrpcWpfClient;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
    }

    private async void OnSayHelloClick(object sender, RoutedEventArgs e)
    {
        this.ResponseTextBlock.Text = "Calling gRPC backend...";

        if (!Uri.TryCreate(this.BackendAddressTextBox.Text.Trim(), UriKind.Absolute, out var address)
            || address.Scheme != Uri.UriSchemeHttps)
        {
            this.ResponseTextBlock.Text = "Enter a valid HTTPS backend address.";
            return;
        }

        try
        {
            using var channel = GrpcChannel.ForAddress(address);
            var client = new Greeter.GreeterClient(channel);
            var response = await client.SayHelloAsync(new HelloRequest { Name = this.NameTextBox.Text });
            this.ResponseTextBlock.Text = response.Message;
        }
        catch (RpcException exception)
        {
            this.ResponseTextBlock.Text = $"gRPC call failed ({exception.StatusCode}): {exception.Status.Detail}";
        }
    }
}
