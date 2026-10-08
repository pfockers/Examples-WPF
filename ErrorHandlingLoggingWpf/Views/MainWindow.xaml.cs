using System.Windows;
using ErrorHandlingLoggingWpf.ViewModels;

namespace ErrorHandlingLoggingWpf.Views;

public partial class MainWindow
{
    private readonly MainWindowViewModel _viewModel;

    public MainWindow(MainWindowViewModel viewModel)
    {
        this.InitializeComponent();
        this._viewModel = viewModel;
        this.DataContext = this._viewModel;
    }

    private void OnRunSuccessClick(object sender, RoutedEventArgs e) => this._viewModel.RunSuccessfulOperation();

    private void OnHandledFailureClick(object sender, RoutedEventArgs e) => this._viewModel.RunHandledFailure();

    private void OnUnhandledFailureClick(object sender, RoutedEventArgs e) => this._viewModel.ThrowUnhandledFailure();
}
