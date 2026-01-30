namespace DICOMApp.Data
{

    /// <summary>
    /// Klasa przechowująca metadane DICOM obrazu:
    ///     * ID pacjenta
    ///     * Nazwa pacjenta
    ///     * Data badania
    ///     * UID badania
    ///     * UID obrazu
    /// </summary>
    public class DICOMMetadata
    {
        /// <summary>
        /// ID pacjenta
        /// </summary>
        public string PatientID { get; set; }

        /// <summary>
        /// Imię i nazwisko pacjenta
        /// </summary>
        public string PatientName { get; set; }

        /// <summary>
        /// Data badania
        /// </summary>
        public string StudyDate { get; set; }

        /// <summary>
        /// Unikalny identyfikator badania
        /// </summary>
        public string StudyUID { get; set; }

        /// <summary>
        /// Unikalny identyfikator obrazu z badania
        /// </summary>
        public string UID { get; set; }

        /// <summary>
        /// Tworzy domyślną instancję metadanych DICOM z wartościami "Unknown"
        /// </summary>
        public DICOMMetadata()
        {
            PatientID = "Unknown";
            PatientName = "Unknown";
            StudyDate = "Unknown";
            StudyUID = "Unknown";
            UID = "Unknown";
        }
    }
}
