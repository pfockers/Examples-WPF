using System.Windows;
using WpfAdvancedTraining.ViewModels;

namespace WpfAdvancedTraining.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}
