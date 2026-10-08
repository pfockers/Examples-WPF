using System.Windows;
using DependencyInjectionWpf.ViewModels;

namespace DependencyInjectionWpf.Views;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;

    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    private void OnGreetClick(object sender, RoutedEventArgs e) => _viewModel.Greet();
}
