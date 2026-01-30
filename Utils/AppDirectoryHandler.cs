using System;
using System.IO;

namespace DICOMApp.Utils
{
    internal class AppDirectoryHandler
    {

        private static AppDirectoryHandler? _instance = null;

        public static AppDirectoryHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppDirectoryHandler();
                }
                return _instance;
            }
        }

        private readonly string _appDirectory;
        private readonly string _exportDirectory;

        private AppDirectoryHandler() {

            _appDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DICOM_App"
            );

            _exportDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "ExportedDICOMData"
            );

            // utworzenie wszystkich katalogów
            Directory.CreateDirectory(_exportDirectory);
            Directory.CreateDirectory(_appDirectory);
            Directory.CreateDirectory(GetExportedImagesDirectory());
            Directory.CreateDirectory(GetDatabaseDirecory());
            Directory.CreateDirectory(GetDicomDirectory());
            Directory.CreateDirectory(GetONNXModelDirectory());
            Directory.CreateDirectory(GetExportedDataDirectory());
            Directory.CreateDirectory(GetPNGImagesDirectory());
        }

        public string GetDicomDirectory() => 
            Path.Combine(_appDirectory, "DICOM_images");


        public string GetDatabaseDirecory() =>
            Path.Combine(_appDirectory, "Exported_images");


        public string GetExportedZIPFilename() => 
            _exportDirectory + ".zip";


        public string GetExportedImagesDirectory() => 
            Path.Combine(_exportDirectory, "images");

        public string GetPNGImagesDirectory() => 
            Path.Combine(_appDirectory, "PNG_images");


        public string GetExportedDataDirectory() => 
            _exportDirectory;


        public string GetONNXModelDirectory() => 
            Path.Combine(_appDirectory, "ONNX_models");

    }
}
