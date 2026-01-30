using DICOMApp.Utils;
using System.Collections.Generic;

namespace DICOMApp.Repository
{
    /// <summary>
    /// Klasa obsługująca eksport danych do pliku CSV
    /// </summary>
    public class CSVExportHander
    {
        private string _dirPath;
        private string _filePath;
        private string _magnificationDataFilePath;

        /// <summary>
        /// Inicjalizuje handler eksportu danych do pliku CSV
        /// </summary>
        public CSVExportHander()
        {
            _dirPath = AppDirectoryHandler.Instance.GetExportedDataDirectory();
            _filePath = System.IO.Path.Combine(_dirPath, "exported_data.csv");
            _magnificationDataFilePath = System.IO.Path.Combine(_dirPath, "magnification_data.csv");
        }

        /// <summary>
        /// Zapisuje wyeksportowane dane do pliku CSV
        /// Zakłada strukturę kolumn:
        /// image_uid, optic_nerve_length, d1x, d1y, d2x, d2y, d3x, d3y, d4x, d4y
        /// </summary>
        /// <param name="rows">Lista wierszy danych do zapisania</param>
        public void WriteExportedImageData(List<List<string>> rows)
        {
            // nagłówki kolumn
            var headers = new List<string>
            {
                "image_uid",
                "optic_nerve_length",
                "d1x", "d1y",
                "d2x", "d2y",
                "d3x", "d3y",
                "d4x", "d4y",
            };

            WriteToCSV(headers, rows, _filePath);
        }

        /// <summary>
        /// Zapisuje dane dotyczące magnifikacji do pliku CSV
        /// Zakłada strukturę kolumn:
        /// image_uid, center_x, center_y
        /// </summary>
        /// <param name="rows">Lista wierszy danych do zapisania</param>
        public void WriteMagnificationData(List<List<string>> rows)
        {
            var headers = new List<string>
            {
                "image_id",
                "center_x",
                "center_y"
            };

            WriteToCSV(headers, rows, _magnificationDataFilePath);
        }

        /// <summary>
        /// Zapisuje dane do pliku CSV
        /// </summary>
        /// <param name="headers">Lista nagłówków kolumn</param>
        /// <param name="rows">Lista wierszy danych</param>
        /// <param name="filePath">Ścieżka do pliku CSV</param>
        public void WriteToCSV(List<string> headers, List<List<string>> rows, string filePath)
        {
            var csvLines = new List<string>();

            // Dodaj nagłówki
            csvLines.Add(string.Join(",", headers));

            // Dodaj wiersze danych
            foreach (var row in rows)
            {
                csvLines.Add(string.Join(",", row));
            }

            // Zapisz do pliku
            System.IO.File.WriteAllLines(filePath, csvLines);
        }
    }
}
