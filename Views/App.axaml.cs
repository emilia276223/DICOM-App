using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DICOMApp.Other;
using FellowOakDicom;
using FellowOakDicom.Imaging.Codec;
using FellowOakDicom.Imaging.NativeCodec;

//using FellowOakDicom.Imaging.Desktop;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace DICOMApp.Views;

/// <summary>
/// Klasa główna aplikacji
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Metoda inicjalizująca aplikację
    /// </summary>
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// Metoda wywoływana na zakończenie inicjalizacji aplikacji
    /// </summary>
    public override void OnFrameworkInitializationCompleted()
    {
        // inicjalizacja FellowOakDicom
        new DicomSetupBuilder()
               .RegisterServices(s =>
               {
                   s.AddFellowOakDicom();
                   s.AddTranscoderManager<NativeTranscoderManager>();
               })
               .SkipValidation()
               .Build();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }


        base.OnFrameworkInitializationCompleted();
    }
}