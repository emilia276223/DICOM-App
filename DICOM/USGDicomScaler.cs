using FellowOakDicom;
using System;
using Avalonia;
using DICOMApp.Interfaces;

namespace DICOMApp.DICOM
{

    /// <summary>
    /// Klasa obsługująca skalowanie DICOM dla obrazów ultrasonograficznych
    /// implementująca interfejs IDicomScaler
    /// </summary>
    public class USGDICOMScaler : IDicomScaler
    {
        private DicomDataset _region;
        private double _deltaY;
        private double _deltaX;


        private double _unitsX;
        private double _unitsY;


        /// <summary>
        /// Tworzy skaler dla pliku dicom na postawie podanych metadanych pliku
        /// </summary>
        /// <param name="dataset">Dataset z metadanych pliku</param>
        /// <param name="regions">Regiony z metadanych pliku</param>
        public USGDICOMScaler(DicomDataset dataset, DicomSequence regions)
        {
            _region = regions.Items[0];

            _deltaY = _region.GetSingleValue<double>(DicomTag.PhysicalDeltaY);
            _deltaX = _region.GetSingleValue<double>(DicomTag.PhysicalDeltaX);

            _unitsX = _region.GetSingleValue<double>(DicomTag.PhysicalUnitsXDirection);
            _unitsY = _region.GetSingleValue<double>(DicomTag.PhysicalUnitsYDirection);

            checkUnitName();
        }


        /// <summary>
        /// Sprawdza, czy Unit Name w metadanych pliku jest cm - jedyna możliwa wartość dla odległości
        /// </summary>
        /// <exception cref="Exception">Wyrzucane, gdy unit są inne niż cm</exception>
        private void checkUnitName()
        {
            //0 - None or not applicable
            //1 - Percent
            //2 - dB
            //3 - cm
            //4 - seconds
            //5 - hertz(seconds ^ (-1))
            //6 - dB / seconds
            //7 - cm / sec
            //8 - cm2
            //9 - cm2 / sec
            //10 - cm3
            //11 - cm3 / sec
            //12 - degrees
            if (_unitsX != 3 || _unitsY != 3)
            {
                throw new Exception("Units of DICOM not representing length");
            }
        }

        /// <inheritdoc cref="IDicomScaler.GetActualLength(Point, Point)"/>
        public double GetActualLength(Point start, Point end)
        {
            // jeśli prawie pionowa to wystarczy, ze w pionie policzymy
            var x_len = Math.Abs(end.X - start.X) * _deltaX;
            var y_len = Math.Abs(end.Y - start.Y) * _deltaY;

            var len_in_cm = Math.Sqrt(
                                Math.Pow(x_len, 2)
                                + Math.Pow(y_len, 2));

            return len_in_cm * 10; // bo w mm chcemy
        }
    }
}
