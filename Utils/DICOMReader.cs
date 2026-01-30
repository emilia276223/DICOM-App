
using DICOMApp.Data;
using System;
using System.IO;
using FellowOakDicom;

namespace DICOMApp.Utils
{
    /// <summary>
    /// Klasa do czytania plików DICOM i wydobywania z nich danych oraz metadanych
    /// </summary>
    public class DICOMReader
    {
        /// <summary>
        /// Zwraca obraz z podanego pliku DICOM  oraz jego dane 
        /// w postaci obiektu DICOMImageData
        /// </summary>
        /// <param name="dicomFilePath">Ścieżka do pliku DICOM</param>
        /// <returns></returns>
        public static DICOMImageData? GetDICOMImageData(string dicomFilePath)
        {
            // sprawdzenie czy plik istnieje
            if (dicomFilePath == null || !File.Exists(dicomFilePath))
            {
                return null;
            }

            // wczytanie pliku DICOM
            DicomFile dicomFile;
            try
            {
                dicomFile = DicomFile.Open(dicomFilePath);
                if (dicomFile == null)
                    return null;
            }
            catch { return null; }

            // odczytanie metadanych i obrazu
            var metadata = GetImageMetadata(dicomFile);
            var imagedata = GetImage(dicomFile);

            if (imagedata == null || metadata == null)
            {
                return null;
            }

            // tworzenie obiektu DICOMImageData
            return new DICOMImageData(metadata, imagedata);
        }


        /// <summary>
        /// Zwraca metadane DICOM z podanego pliku w postaci obiektu DICOMMetadata
        /// </summary>
        /// <param name="dicomFile">Wczytany plik DICOM (DicomFile)</param>
        /// <returns>DICOMMetadata z odczytanymi danymi</returns>
        /// <exception cref="System.IO.FileNotFoundException">Wyrzucany gdy plik DICOM nie istnieje</exception>
        /// <exception cref="Exception">Wyrzucany gdy wystąpi błąd podczas odczytu pliku DICOM</exception>
        private static DICOMMetadata? GetImageMetadata(DicomFile dicomFile)
        {
            try
            {
                var patientName = dicomFile.Dataset.GetSingleValueOrDefault(DicomTag.PatientName, "Unknown");
                var savedName = patientName;
                if (patientName.Contains("^"))
                    savedName = patientName.Split('^')[0] + " " + patientName.Split('^')[1];

                var studyDate = dicomFile.Dataset.GetSingleValueOrDefault(DicomTag.StudyDate, "Unknown");
                var savedDate = studyDate;
                if(!studyDate.Contains("."))
                    savedDate = studyDate.Substring(0, 4) + "." + studyDate.Substring(4, 2) + "." + studyDate.Substring(6, 2);

                var metadata = new DICOMMetadata
                {
                    PatientID = dicomFile.Dataset.GetSingleValueOrDefault(DicomTag.PatientID, "Unknown"),
                    PatientName = savedName,
                    StudyDate = savedDate,
                    StudyUID = dicomFile.Dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, "Unknown"),
                    UID = dicomFile.Dataset.GetSingleValueOrDefault(DicomTag.SOPInstanceUID, "Unknown"),
                };

                return metadata;
            }
            catch { return null; }
        }

        /// <summary>
        /// Przycina obraz do wymiarów 560x680, dodając padding jeśli oryginał jest mniejszy
        /// </summary>
        /// <param name="original">Pixele oryginalnego obrazu</param>
        /// <param name="originalWidth">Szerokość oryginalnego obrazu</param>
        /// <param name="originalHeight">Wysokość oryginalnego obrazu</param>
        /// <returns>Krotka z przyciętymi pixelami, nową szerokością i wysokością</returns>
        private static (int[], int, int) CropWithAddingPadding(int[] original, int originalWidth, int originalHeight)
        {
            int width = 560;
            int height = 680;
            int[] res = new int[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // jeśli poza obrazem to ustawiamy na 0
                    if (x >= originalWidth || y >= originalHeight)
                    {
                        res[y * width + x] = 0;
                    }

                    // kopiujemy piksel z oryginału do nowej tablicy
                    else
                    {
                        res[y * width + x] = original[y * originalWidth + x];
                    }
                }
            }
            return (res, width, height);
        }

        /// <summary>
        /// Przycina obraz do wymiarów 560x680 zgodnie z ustalonymi współrzędnymi
        /// </summary>
        /// <param name="original">Pixele oryginalnego obrazu</param>
        /// <param name="originalHeight">Szerokość oryginalnego obrazu</param>
        /// <param name="originalWidth">Wysokość oryginalnego obrazu</param>
        /// <returns>Krotka z przyciętymi pixelami, nową szerokością i wysokością</returns>
        private static (int[], int, int) CropPixels(int[] original, int originalWidth, int originalHeight)
        {
            // jeśli obraz ma inne wymiary niż przewidziane to przycinamy tak, żeby się zgadzało wielkością
            if (originalWidth < 890 || originalHeight < 735)
            {
                return CropWithAddingPadding(original, originalWidth, originalHeight);
            }

            // chcemy przyciąć do samego obrazu (bez marginesów)
            // interesują nas piksele: w pionie od 55 do 735, w poziomie od 330 do 890
            var startX = 330;
            var endX = 890;
            var startY = 55;
            var endY = 735;

            int width = endX - startX; // 560
            int height = endY - startY; // 680
            int[] cropped = new int[width * height];


            // kopiujemy piksele z oryginału do nowej tablicy
            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    cropped[(y - startY) * width + (x - startX)] = original[y * originalWidth + x];
                }
            }

            return (cropped, width, height);
        }


        /// <summary>
        /// Wczytuje plik DICOM i zwraca przycięty obraz jako ImageData
        /// </summary>
        /// <param name="dicomFile">DicomFile</param>
        /// <returns>Instancję ImageData z przyciętym obrazem</returns>
        /// <exception cref="Exception">Wyrzucany gdy plik nie jest poprawnym DICOMem</exception>
        private static ImageData? GetImage(DicomFile dicomFile)
        {
            try {
                var dicomImage = new FellowOakDicom.Imaging.DicomImage(dicomFile.Dataset);
                var rendered = dicomImage.RenderImage();


                int[] src = rendered.Pixels.Data;

                int originalWidth = rendered.Width;
                int originalHeight = rendered.Height;

                int[] cropped;
                int newWidth, newHeight;


                (cropped, newWidth, newHeight) = CropPixels(src, originalWidth, originalHeight);
                return new ImageData(newWidth, newHeight, cropped);
            }
            catch { return null; }
        }

    }
}
