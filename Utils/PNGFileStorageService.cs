using DICOMApp.Data;
using System.IO;

namespace DICOMApp.Utils
{
    internal class PNGFileStorageService
    {
        private static string GetExportPath(string imageGUID)
            => Path.Combine(
                AppDirectoryHandler.Instance.GetExportedImagesDirectory()
                , imageGUID + ".png");


        private static string GetPath(string imageID) 
            => Path.Combine(
                AppDirectoryHandler.Instance.GetPNGImagesDirectory()
                , imageID + ".png");

        /// <summary>
        /// Metoda zapisująca obraz ImageData do pliku PNG
        /// W przypadku gdy plik istnieje, metoda nic nie robi
        /// </summary>
        /// <param name="image">Obraz, który zapisujemy do pliku PNG</param>
        /// <param name="imageID">ID obrazu, który zapisujemy do pliku PNG</param>
        public static void SaveImageDataToPNG(ImageData image, string imageID)
        {
            var path = GetPath(imageID);

            // jeśli plik istnieje to nic nie robimy
            if (File.Exists(path))
                return;

            var bitmap = image.GetBitmap();
            using (var stream = File.OpenWrite(path))
            {
                bitmap.Save(stream);
            }
        }

        public static void ExportSavedPNGImage(string imageID, string imageGUID)
        {
            if (!File.Exists(GetPath(imageID))) return;
            File.Copy(GetPath(imageID), GetExportPath(imageGUID), true);
        }


        /// <summary>
        /// Metoda zwracająca obraz zapisany do pliku PNG jako Bitmap
        /// </summary>
        /// <param name="ImageID">ID zapisanego obrazu</param>
        /// <returns>Bitmapa jeśli plik był zapisany, w przeciwnym przypadku null</returns>
        public static Avalonia.Media.Imaging.Bitmap? GetBitmapOfSavedImage(string ImageID)
        {
            var path = GetPath(ImageID);
            if (!File.Exists(path)) return null;
            try
            {
                return new Avalonia.Media.Imaging.Bitmap(path);
            }
            catch { return null; }
        }
    }
}
