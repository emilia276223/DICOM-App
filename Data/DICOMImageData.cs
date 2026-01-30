using System;
using System.Security.Cryptography;
using System.Text;

namespace DICOMApp.Data
{
    /// <summary>
    /// Klasa przechowująca obraz i jego metadane
    /// </summary>
    public class DICOMImageData
    {
        /// <summary>
        /// Metadane obrazu
        /// </summary>
        public DICOMMetadata Metadata;

        /// <summary>
        /// Obraz zapisany jako (ImageData)
        /// </summary>
        public ImageData Image;

        /// <summary>
        /// Wygenerowany identyfikator obrazu
        /// </summary>
        public string ImageID;

        /// <summary>
        /// Konstruktor obrazu, na podstawie metadanych i obrazu
        /// Generuje identyfikator obrazu
        /// </summary>
        /// <param name="metadata">Metadane obrazu DICOM</param>
        /// <param name="image">Obraz DICOM</param>
        public DICOMImageData(DICOMMetadata metadata, ImageData image)
        {
            Metadata = metadata;
            Image = image;
            ImageID = GenerateStableHash(metadata.UID);
        }

        /// <summary>
        /// Generowanie hasha na podstawie podanego stringa
        /// </summary>
        /// <param name="input">napis, którego hash jest generowany</param>
        /// <returns>napis przedstawiający hash</returns>
        private string GenerateStableHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToHexString(bytes).Substring(0, 16); // Skracamy dla czytelności
            }
        }
    }
}
