using DICOMApp.Data;
using System.Collections.Generic;

namespace DICOMApp.Interfaces
{
    /// <summary>
    /// Interfejs klasy obsługującej zapisywanie i odczytywanie danych
    /// </summary>
    public interface IDataHandler
    {

        /// <summary>
        /// Zapisuje wyznaczone punkty do bazy danych lub pliku
        /// </summary>
        /// <param name="points">Punkty do zapisania</param>
        /// <param name="imageID">ID obrazu, na którym zostały zaznaczone punkty</param>
        /// <param name="opticNerveLen">Wyznaczona długość nerwu wzrokowego</param>
        /// <param name="info">Metadane DICOM obrazu</param>
        public void SaveChosenPoints(
            List<double> points,
            string imageID,
            double opticNerveLen,
            DICOMMetadata info);

        /// <summary>
        /// Dodaje informacje o obrazie do bazy danych
        /// </summary>
        /// <param name="metadata">Metadane obrazu</param>
        /// <param name="imageID">ID obrazu</param>
        public void AddImageInfo(DICOMMetadata metadata, string imageID);

        /// <summary>
        /// Zwraća listę długości nerwu wzrokowego dla wszystkich badań danego pacjenta
        /// </summary>
        /// <param name="patientID">ID pacjenta</param>
        /// <returns>Lista krotek (ID badania, długość nerwu wzrokowego)</returns>
        public List<(string, double)> GetOpticNerveLenValuesForStudiesOfPatient(string patientID);

        /// <summary>
        /// Zwraća listę srodków okanów powększających dla wszystkich obrazów danego pacjenta
        /// </summary>
        /// <returns></returns>
        public List<List<string>> GetMagnifiedCentersData();

        /// <summary>
        /// Zwraća listę list zawierający zanonimizowane dane dotyczące zaznaczeń na obrazach
        /// </summary>
        /// <returns></returns>
        public List<List<string>> GetAnonimizedData();

        /// <summary>
        /// Zapisuje do obrazu informacje o srodku okna powększającego
        /// </summary>
        /// <param name="imageID">ID obrazu, którego dane powięszenia zapisujemy</param>
        /// <param name="x">Srodek okan powększającego w osi x</param>
        /// <param name="y">Srodek okan powększającego w osi y</param>
        public void SaveMagnifiedImageCenterInfo(string imageID, double x, double y);

        /// <summary>
        /// Zamyka połączenie z bazą danych
        /// </summary>
        public void CloseConnection();
    }
}
