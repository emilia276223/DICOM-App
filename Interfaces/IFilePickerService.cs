using System.Collections.Generic;
using System.Threading.Tasks;

namespace DICOMApp.Interfaces
{
    /// <summary>
    /// Interfejs obslugujacy wybor plikow
    /// </summary>
    public interface IFilePickerService
    {
        /// <summary>
        /// Metoda umożliwiająca wybranie pliku ONNX
        /// Otwiera okno do wyboru pliku (explorator plików)
        /// </summary>
        /// <returns>Ścieżka do wybranego przez użytkownika pliku lub null</returns>
        Task<string?> PickONNXFileAsync();

        /// <summary>
        /// Metoda umożliwiająca wybranie pliku DICOM
        /// Otwiera okno do wyboru pliku (explorator plików)
        /// </summary>
        /// <returns>Ścieżka do wybranego przez użytkownika pliku lub null</returns>
        Task<string?> PickImageFileAsync();

        /// <summary>
        /// Metoda umożliwiająca wybranie folderu z plikami DICOM
        /// Otwiera okno do wyboru folderu (explorator plików)
        /// </summary>
        /// <returns>Ścieżka do wybranego przez użytkownika folderu lub null</returns>
        Task<List<string>> PickDicomDirectory();
    }
}
