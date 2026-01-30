using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace DICOMApp.Utils
{
    /// <summary>
    /// Obsługuje katalog, w którym są przechowywane pliki DICOM
    /// </summary>
    public class DICOMDirectoryHandler
    {

        /// <summary>
        /// Konstruuje pełną ścieżkę do pliku DICOM na podstawie ID pacjenta, daty badania i nazwy pliku
        /// </summary>
        /// <param name="patientID">ID pacjenta</param>
        /// <param name="studyDate">Data badania</param>
        /// <param name="filename">nazwa pliku</param>
        /// <returns></returns>
        internal static string GetFullImagePath(string patientID, string studyDate, string filename)
        {
            string dicomDirectory = AppDirectoryHandler.Instance.GetDicomDirectory();
            string fullPath = Path.Combine(dicomDirectory,
                patientID,
                studyDate,
                filename
            );
            return fullPath;
        }


        private static string GetPathForMetadata(string patientID, string studyDate, string uid)
        {
            string dicomDirectory = AppDirectoryHandler.Instance.GetDicomDirectory();
            // twotzymy katalogi
            string destPath = Path.Combine(
                dicomDirectory,
                patientID,
                studyDate);
            Directory.CreateDirectory(destPath);
            destPath = Path.Combine(destPath, uid + ".dcm");
            return destPath;
        }

        /// <summary>
        /// Dodaje plik DICOM do katalogu DICOM
        /// </summary>
        /// <param name="sourcePath">Nazwa pliku źródłowego</param>
        /// <param name="patientID">ID pacjenta</param>
        /// <param name="studyDate">Data badania</param>
        /// <param name="uid">ID unikalny pliku DICOM</param>
        internal static void AddDICOMFile(string sourcePath, string patientID, string studyDate, string uid)
        {
            var destPath = GetPathForMetadata(patientID, studyDate, uid);

            // zeby nie było konfliktów nazw plików, dodajemy uid
            try
            {
                File.Copy(sourcePath, destPath, false);
            }
            catch (Exception ex)
            {
                // plik już istnieje
                // to nie problem, więc nic nie robimy
                Console.WriteLine($"Error copying DICOM file: {ex.Message}");
            }
        }

        /// <summary>
        /// Zwraca listę badań dla danego pacjenta na podstawie jego ID
        /// </summary>
        /// <param name="patientID">ID pacjenta</param>
        /// <returns>Lista napisów zawierających daty badań pacjenta</returns>
        internal static ObservableCollection<string> GetStudiesForPatient(string patientID)
        {
            string dicomDirectory = AppDirectoryHandler.Instance.GetDicomDirectory();
            var studiesDir = Path.Combine(dicomDirectory, patientID);
            if (!Directory.Exists(studiesDir))
            {
                return new ObservableCollection<string>();
            }
            var dirs = Directory.GetDirectories(studiesDir);
            return new ObservableCollection<string>(
                Enumerable.Select(
                    dirs,
                    d => Path.GetFileName(d)
                )
            );
        }

        /// <summary>
        /// Zwraca listę obrazów dla danego badania pacjenta
        /// </summary>
        /// <param name="patientID">ID pacjenta</param>
        /// <param name="studyID">ID badania</param>
        /// <returns>Lista nazw plików obrazów w badaniu</returns>
        internal static ObservableCollection<string> GetImagesForStudy(string patientID, string studyID)
        {

            string dicomDirectory = AppDirectoryHandler.Instance.GetDicomDirectory();
            var imagesDir = Path.Combine(
                dicomDirectory, 
                patientID,
                studyID);

            if (!Directory.Exists(imagesDir))
            {
                Console.Error.WriteLine($"Directory does not exist: {imagesDir}");
                return new ObservableCollection<string>(["nie ma katalogu"]);
            }
            var images = Directory.GetFiles(imagesDir);
            return new ObservableCollection<string>(
                Enumerable.Select(
                    images,
                    d =>Path.GetFileName(d)
                )
            );
        }

        /// <summary>
        /// Zwraca listę pacjentów w katalogu DICOM
        /// </summary>
        /// <returns>Lista ID pacjentów</returns>
        internal static ObservableCollection<string> GetPatients()
        {
            string dicomDirectory = AppDirectoryHandler.Instance.GetDicomDirectory();
            var dirs = Directory.GetDirectories(dicomDirectory);
            return new ObservableCollection<string>(Enumerable.Select(
                    dirs,
                    d => Path.GetFileName(d)));
        }
    }
}
