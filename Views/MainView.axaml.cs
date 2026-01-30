using Avalonia.Controls;
using Avalonia.Platform.Storage;
using DICOMApp.Other;

namespace DICOMApp;

/// <summary>
/// Główne okno aplikacji
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Konstruktor głównego okna aplikacji
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new ViewModel.MainViewModel(
                new FilePickerService(StorageProvider)
            );

        this.Closed += (_, _) =>
        {
            (DataContext as ViewModel.MainViewModel)?.Close();
        };
    }

    private void InitializeComponent()
    {
        Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
    }
}