using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMApp.Interfaces
{
    internal interface IExportHandler
    {
        /// <summary>
        /// Metoda eksportująca dane
        /// </summary>
        /// <returns>True jeśli dane zostały wyeksportowane, w przeciwnym przypadku false</returns>
        public bool ExportData();
    }
}
