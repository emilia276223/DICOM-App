using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Avalonia.Platform.Storage;
using DICOMApp.Interfaces;

namespace DICOMApp.Other
{
    /// <summary>
    /// Klasa obslugujaca wybieranie plików
    /// Za pomocą eksploratora plików umożliwia wybor plików
    /// </summary>
    /// <remarks>
    /// Implementuje interfejs IFilePickerService
    /// </remarks>
    public class FilePickerService : IFilePickerService
    {
        private readonly IStorageProvider _sp;

        /// <summary>
        /// Konstruktor klasy
        /// </summary>
        /// <param name="sp">Obiekt implementujacy interfejs IStorageProvider</param>
        public FilePickerService(IStorageProvider sp)
        {
            _sp = sp;
        }

        /// <summary>
        /// Metoda zwracająca wszystkie pliki z folderu
        /// </summary>
        /// <param name="dir">Ścieżka do folderu</param>
        /// <returns>Lista ścieżek do wszystkich plików</returns>
        private List<string> GetAllFilesFromDirectory(string dir)
        {
            var files = new List<string>();

            // nie powinno się wydarzyć, ale na wszelki wypadek można sprawdzić
            if (!Directory.Exists(dir))
            {
                return files;
            }

            foreach (var file in Directory.GetFiles(dir))
            {
                files.Add(file);
            }
            return files;
        } 
        
        /// <summary>
        /// Metoda umożliwiająca wybranie folderu z plikami DICOM
        /// </summary>
        /// <param name="startPath">Ścieżka do folderu z plikami DICOM lub null</param>
        /// <returns>Lista ścieżek do plików DICOM</returns>
        private async Task<List<string>> PickFolderAsync(string? startPath)
        {
            if (startPath == null)
            {
                startPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }

            var options = new FolderPickerOpenOptions
            {
                Title = "Select DICOM folder",
                AllowMultiple = false,
                SuggestedStartLocation = await _sp.TryGetFolderFromPathAsync(startPath),
            };

            var dir = await _sp.OpenFolderPickerAsync(options);

            // jeśli nie wyszło zwracamy pustą listę
            if(dir == null)
            {
                return new List<string>();
            }

            // zwracamy wszystkie pliki z wybranego katalogu
            var dirPath = dir.FirstOrDefault()?.Path.LocalPath;
            return GetAllFilesFromDirectory(dirPath);
        }

        /// <summary>
        /// Metoda umożliwiająca wybranie pliku z wybranego folderu
        /// </summary>
        /// <param name="startPath">Ścieżka do katalogu, który ma zostać otworzony domyślnie</param>
        /// <param name="fileType">Typ pliku, który ma zostać wybrany</param>
        /// <param name="patterns">Lista możliwych rozszerzeń pliku</param>
        /// <returns></returns>
        private async Task<string?> PickFileAsync(string? startPath, string fileType, List<string> patterns)
        {
            if (startPath == null)
            {
                startPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }

            var options = new FilePickerOpenOptions
            {
                Title = "Select a file",
                AllowMultiple = false,
                SuggestedStartLocation = await _sp.TryGetFolderFromPathAsync(startPath),
                FileTypeFilter =
                [
                    new FilePickerFileType(fileType)
                    {
                        Patterns = patterns
                    }

                ]
            };

            var files = await _sp.OpenFilePickerAsync(options);
            return files.FirstOrDefault()?.Path.LocalPath;
        }

        /// <summary>
        /// Metoda umożliwiająca wybranie pliku typu ONNX
        /// Domyślny folder to katalog z pobranymi plikami
        /// Umożliwia wybranie tylko plików typu ONNX (*.onnx)
        /// </summary>
        /// <returns>Ścieżkę do wybranego pliku lub null</returns>
        public async Task<string?> PickONNXFileAsync()
        {

            var downloadsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads");

            return await PickFileAsync(downloadsPath, "ONNX files", ["*.onnx"]);
        }

        /// <summary>
        /// Metoda umożliwiająca wybranie pliku obrazu
        /// Domyślny folder to katalog z pobranymi plikami
        /// Umożliwia wybranie tylko plików o rozszerzeniach z listy:
        ///     *.png, *.jpeg, *.jpg, *.dcm
        ///     *.* - wszystkie pliki
        /// </summary>
        /// <returns>Ścieżka do wybranego pliku</returns>
        public async Task<string?> PickImageFileAsync()
        {
            var downloadsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads");
            return await PickFileAsync(downloadsPath, "Image files", ["*.png", "*.jpeg", "*.jpg", "*.dcm", "*.*"]);
        }

        /// <summary>
        /// Metoda umożliwiająca wybranie katalogu z plikami DICOM
        /// </summary>
        /// <returns>Lista ścieżek do plików z wybranego katalogu</returns>
        public async Task<List<string>> PickDicomDirectory()
        {
            var downloadsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads");
            return await PickFolderAsync(downloadsPath);
        }
    }
}
