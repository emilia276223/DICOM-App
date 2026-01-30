using Avalonia;

namespace DICOMApp.Interfaces
{
    /// <summary>
    /// Interfejs klasy obsługującej skalowanie DICOM
    /// </summary>
    public interface IDicomScaler
    {
        /// <summary>
        /// Wylicza rzeczywistą długość między dwoma punktami na obrazie DICOM
        /// </summary>
        /// <param name="start">Punkt (Avalonia Point) początku odcinka do pomiaru</param>
        /// <param name="end">Punkt (Avalonia Point) końca odcinka do pomiaru</param>
        /// <returns>Długość rzeczywista między dwoma punktami w milimetrach</returns>
        public double GetActualLength(Point start, Point end);
    }
}
