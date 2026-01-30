using FellowOakDicom;
using DICOMApp.Interfaces;
using DICOMApp.DICOM;

namespace DICOMApp.Utils
{
    /// <summary>
    /// Fabryka tworząca odpowiedni skaler DICOM w zależności od danych zawartych w pliku DICOM
    /// </summary>
    internal class DICOMScalerFactory
    {
        /// <summary>
        /// Tworzy odpowiedni skaler DICOM na podstawie danych zawartych w pliku DICOM
        /// </summary>
        /// <remarks>
        /// W przypadku gdy plik DICOM zawiera tagi Pixel Spacing lub Imager Pixel Spacing
        /// zwracany jest LinearDICOMScaler.
        /// W przypadku gdy plik DICOM zawiera tag Sequence of Ultrasound Regions
        /// zwracany jest USGDICOMScaler.
        /// W przeciwnym wypadku zwracany jest LinearDICOMScaler z zerowymi skalami.
        /// </remarks>
        /// <param name="filePath">Plik DICOM do analizy</param>
        /// <returns>Skaler DICOM implementujący interfejs IDicomScaler</returns>
        public static IDicomScaler GetDICOMScaler(string filePath)
        {
            var _filePath = filePath;
            var _dicomFile = DicomFile.Open(filePath);
            var _dataset = _dicomFile.Dataset;

            double rowScale;
            double colScale;
            if (_dataset.TryGetValues<double>(DicomTag.PixelSpacing, out double[] ps))
            {
                rowScale = ps[0];
                colScale = ps[1];
                return new LinearDICOMScaler(rowScale, colScale);
            }

            if (_dataset.TryGetValues<double>(DicomTag.ImagerPixelSpacing, out ps))
            {
                rowScale = ps[0];
                colScale = ps[1];
                return new LinearDICOMScaler(rowScale, colScale);
            }

            if (_dataset.TryGetSequence(DicomTag.SequenceOfUltrasoundRegions, out var regions))
            {
                return new USGDICOMScaler(_dataset, regions);
            }

            //throw new ArgumentException("dla takiego pliku nie ma skali");
            return new LinearDICOMScaler(0, 0);
        }
    }
}
