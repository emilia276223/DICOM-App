using DICOMApp.Interfaces;
using DICOMApp.Utils;
using LiveChartsCore.SkiaSharpView.Avalonia;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMApp.Repository
{
    internal class DataExportHandler : IExportHandler
    {
        private readonly CSVExportHander _csvHandler = new CSVExportHander();

        private readonly IDataHandler _dbHandler;

        public DataExportHandler(IDataHandler dbHandler) 
        {
            _dbHandler = dbHandler;
        }


        public bool ExportData()
        {
            // wydobywanie danych
            List<List<string>> anonimizedData, magnifiedCentersData;
            try
            {
                anonimizedData = _dbHandler.GetAnonimizedData();
                magnifiedCentersData = _dbHandler.GetMagnifiedCentersData();
            }
            catch { return false; }

            // zrobienie słownika tak, by id obrazów zostały przetłumaczone na oryginalne wartości
            var imagesWithInformationUID = new Dictionary<string, string>();
            for (var i = 0; i < anonimizedData.Count; i++)
            {
                var id = anonimizedData[i][0];
                if (!imagesWithInformationUID.ContainsKey(id))
                {
                    imagesWithInformationUID.Add(id, Guid.NewGuid().ToString());
                }
                anonimizedData[i][0] = (imagesWithInformationUID[id]);
            }
            for (var i = 0; i < magnifiedCentersData.Count; i++)
            {
                var id = magnifiedCentersData[i][0];
                if (!imagesWithInformationUID.ContainsKey(id))
                {
                    imagesWithInformationUID.Add(id, Guid.NewGuid().ToString());
                }
                magnifiedCentersData[i][0] = (imagesWithInformationUID[id]);
            }

            // wyeksportowanie kopii plików PNG reprezentujących obrazy, których dane zostały wyeksportowane
            foreach (var image in imagesWithInformationUID)
            {
                PNGFileStorageService.ExportSavedPNGImage(image.Key, image.Value);
            }

            // zapisanie do pliku
            _csvHandler.WriteExportedImageData(anonimizedData);
            _csvHandler.WriteMagnificationData(magnifiedCentersData);

            // ZIP wyeksportowanych danych
            var sourcePath = AppDirectoryHandler.Instance.GetExportedDataDirectory();
            var zipPath = AppDirectoryHandler.Instance.GetExportedZIPFilename();
            if (System.IO.File.Exists(zipPath))
            {
                System.IO.File.Delete(zipPath);
            }
            System.IO.Compression.ZipFile.CreateFromDirectory(sourcePath, zipPath);

            return true;
        }
    }
}
