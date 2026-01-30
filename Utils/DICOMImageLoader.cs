using DICOMApp.Data;

namespace DICOMApp.Utils
{
    /// <summary>
    /// Klasa do wczytywania i zapisywania obrazów DICOM
    /// Korzysta z klas DICOMReader i PNGFileStorageService
    /// </summary>
    internal class DICOMImageLoader
    {
        /// <summary>
        /// Metoda wczytuje obraz z pliku DICOM, zapisuje go do katalogu i do pliku PNG
        /// Zwraca obiekt DICOMImageData w przypadku sukcesu
        /// </summary>
        /// <param name="file">Ścieżka do pliku DICOM</param>
        /// <returns>Obiekt DICOMImageData w przypadku sukcesu, w przeciwnym przypadku null</returns>
        public static DICOMImageData? LoadAndRegisterDICOMImage(string file) {

            var dicomImageData = DICOMReader.GetDICOMImageData(file);

            // jeśli nie udało się załadować pliku
            if (dicomImageData == null)
                return null;


            // zapisanie pliku do PNG
            PNGFileStorageService.SaveImageDataToPNG(
                dicomImageData.Image,
                dicomImageData.ImageID
            );

            // zapisanie pliku do katalogu
            var meta = dicomImageData.Metadata;
            DICOMDirectoryHandler.AddDICOMFile(file,
                meta.PatientID,
                meta.StudyDate,
                meta.UID);

            return dicomImageData;

        }
    }
}
