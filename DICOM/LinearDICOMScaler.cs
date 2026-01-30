using System;
using Avalonia;
using DICOMApp.Interfaces;

namespace DICOMApp.DICOM
{
    /// <summary>
    /// Klasa obsługująca skalowanie DICOM dla obrazów ze skalą liniową
    /// </summary>
    public class LinearDICOMScaler : IDicomScaler
    {

        private double _rowScale;
        private double _colScale;

        /// <summary>
        /// Tworzy instancję skalera na podstawie skali pionowej i poziomej
        /// </summary>
        /// <param name="rowScale"></param>
        /// <param name="colScale"></param>
        internal LinearDICOMScaler(double rowScale, double colScale)
        {
            _rowScale = rowScale;
            _colScale = colScale;
        }

        /// <inheritdoc cref="IDicomScaler.GetActualLength(Point, Point)"/>
        public double GetActualLength(Point start, Point end)
        {
            var rowLenght = ((double)end.X - start.X) * _rowScale;
            var colLenght = ((double)end.Y - start.Y) * _colScale;

            return Math.Sqrt(
                        Math.Pow(rowLenght, 2)
                        + Math.Pow(colLenght, 2));
        }
    }
}
