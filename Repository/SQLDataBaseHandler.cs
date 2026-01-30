using DICOMApp.Data;
using DICOMApp.Interfaces;
using DICOMApp.Utils;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace DICOMApp.Repository
{
    /// <summary>
    /// Klasa obsługująca bazę danych SQLite do przechowywania informacji o pacjentach, badaniach i obrazach
    /// </summary>
    /// <remarks>
    /// Implementuje wzorzec IDisposable do prawidłowego zarządzania zasobami
    /// Implementuje interfejs IDataHandler do obsługi zapisu i odczytu danych
    /// </remarks>
    public class SQLDataBaseHandler : IDataHandler, IDisposable
    {
        private SQLDataBaseConnectionHandler? _sqlConnection;
        private SqliteConnection? _conn;
        private bool disposedValue;


        /// <summary>
        /// Tworzy handler bazy danych SQLite
        /// </summary>
        public SQLDataBaseHandler()
        {
            // katalog do przechowywania bazy danych
            var dirPath = AppDirectoryHandler.Instance.GetDatabaseDirecory();


            // otwarcie polaczenia z baza danych
            _sqlConnection = new SQLDataBaseConnectionHandler(
                dirPath,
                Path.Combine(dirPath, "sql_database.db")
            );

            _conn = _sqlConnection.OpenConnection();

            SQLDataBaseSchemaHandler.InitializeDatabase(_conn);
        }

        /// <summary>
        /// Zamyka połączenie z bazą danych SQLite
        /// </summary>
        public void CloseConnection()
        {
            if (_sqlConnection != null)
            {
                _sqlConnection.Dispose();
                _sqlConnection = null;
            }
            _conn = null;
        }

        /// <summary>
        /// Zapisuje do obrazu informacje o srodku okna powększającego
        /// </summary>
        /// <param name="imageID">ID obrazu, którego dane powięszenia zapisujemy</param>
        /// <param name="x">Srodek okan powększającego w osi x</param>
        /// <param name="y">Srodek okan powększającego w osi y</param>
        public void SaveMagnifiedImageCenterInfo(string imageID, double x, double y)
        {
            if (_conn == null) return;
            try
            {
                _conn.Open();
                SQLDataBaseInsertsHandler.InsertMagnifiedImageCenterInfo(_conn, imageID, x, y);
                _conn.Close();
            }
            catch { }
        }

        /// <inheritdoc cref="IDataHandler.SaveChosenPoints">
        public void SaveChosenPoints(List<double> points, string imageID, double opticNerveLen, DICOMMetadata info)
        {
            // sprawdzenie poprawnosci danych
            if (points.Count != 8)
            {
                return; // nie ma co rzucać wyjątku, po prostu nie zapisujemy
            }

            try
            {
                if (_conn == null) return;
                _conn.Open();

                // dodanie pacjenta jesli nie istnieje w bazie
                SQLDataBaseInsertsHandler.InsertPatient(_conn, info.PatientID, info.PatientName);

                // dodanie badania jesli nie istnieje w bazie
                SQLDataBaseInsertsHandler.InsertStudy(_conn, info.StudyUID, info.PatientID, info.StudyDate);

                // dodanie obrazu jesli nie istnieje w bazie
                SQLDataBaseInsertsHandler.InsertImage(_conn, imageID, info.StudyUID);

                // nadpisanie punktow i dlugosci nerwu wzrokowego
                SQLDataBaseInsertsHandler.InsertImageInfo(_conn, points, imageID, opticNerveLen);

                _conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        /// <summary>
        /// Zwraća listę długości nerwu wzrokowego dla wszystkich badań danego pacjenta
        /// </summary>
        /// <param name="patientID">ID pacjenta</param>
        /// <returns>
        /// Lista krotek (data badania, długość nerwu wzrokowego)
        /// </returns>
        public List<(string,double)> GetOpticNerveLenValuesForStudiesOfPatient(string patientID)
        {
            try
            {
                if (_conn == null) return new List<(string, double)>();
                _conn.Open();
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandText =
                        """
                        SELECT study_id, studies.date, MAX(optic_nerve_size)
                        FROM images
                        JOIN studies ON studies.id = images.study_id
                        WHERE optic_nerve_size IS NOT NULL
                        AND studies.patient_id = @pid
                        GROUP BY study_id
                        """;
                    cmd.Parameters.AddWithValue("pid", patientID);


                    var res = new List<(string, double)>();
                    using (var resReader = cmd.ExecuteReader())
                    {
                        while (resReader.Read())
                        {
                            var studyId = resReader.GetString(0);
                            var studyDate = resReader.GetString(1);
                            var maxSize = resReader.GetDouble(2);

                            res.Add((studyDate, maxSize));
                        }
                    }
                    _conn.Close();
                    return res;
                }
            }
            catch (SqliteException)
            {
                return new List<(string, double)>();
            }
        }

        /// <inheritdoc cref="IDataHandler.GetAnonimizedData"/>
        public List<List<string>> GetAnonimizedData()
        {
            if (_conn == null) return new List<List<string>>();
            _conn.Open();
            var res = new List<List<string>>();
            using (var cmd = _conn.CreateCommand())
            {
                // wybieramy tylko obrazy z pełnymi informacjami
                cmd.CommandText =
                    """
                        SELECT id, optic_nerve_size, d1x, d1y, d2x, d2y, d3x, d3y,  d4x, d4y
                        FROM images
                        WHERE optic_nerve_size IS NOT NULL
                        AND d1x IS NOT NULL
                        AND d2x IS NOT NULL
                        AND d3x IS NOT NULL
                        AND d4x IS NOT NULL
                        AND d1y IS NOT NULL
                        AND d2y IS NOT NULL
                        AND d3y IS NOT NULL
                        AND d4y IS NOT NULL
                        """;

                // stworzenie linii do zapisania do csv
                using (var resReader = cmd.ExecuteReader())
                {
                    while (resReader.Read())
                    {
                        // jeśli są to wartości to dodajemy je do listy
                        List<string> data = [
                            resReader.GetString(0), // imageID
                            resReader.GetDouble(1).ToString(CultureInfo.InvariantCulture) // opticNerveSize
                        ]; 

                        for (int i = 2; i <= 9; i++)
                        {
                            data.Add(
                                resReader.GetDouble(i).ToString(CultureInfo.InvariantCulture)); // points
                        }
                        res.Add(data);
                    }
                }
            }
            _conn.Close();
            return res;
        }

        
        /// <inheritdoc cref="IDataHandler.GetMagnifiedCentersData"/>
        public List<List<string>> GetMagnifiedCentersData()
        {
            if (_conn == null) return new List<List<string>>();
            _conn.Open();
            var res = new List<List<string>>();
            using (var cmd = _conn.CreateCommand())
            {
                // wybieramy tylko obrazy z pełnymi informacjami
                cmd.CommandText =
                    """
                        SELECT image_id, center_x, center_y
                        FROM magnified_image_centers
                        WHERE center_x IS NOT NULL
                        AND center_y IS NOT NULL
                        """;

                // stworzenie linii do zapisania do csv
                using (var resReader = cmd.ExecuteReader())
                {
                    while (resReader.Read())
                    {
                        // jeśli są to wartości to dodajemy je do listy
                        res.Add([
                            resReader.GetString(0), // imageID
                            resReader.GetDouble(1).ToString(CultureInfo.InvariantCulture), // center X
                            resReader.GetDouble(2).ToString(CultureInfo.InvariantCulture) // center Y
                        ]);
                    }
                }
            }
            _conn.Close();
            return res;
        }

        


        /// <summary>
        /// Zwalnia zasoby i zamyka połączenie z bazą danych
        /// </summary>
        /// <param name="disposing">Wskaźnik czy metoda została wywołana przez Dispose()</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                CloseConnection();
                disposedValue = true;
            }
        }

        /// <summary>
        /// Zwolnia zasoby zarządzane przez klasę i zamyka połączenie z bazą danych
        /// </summary>
        /// <remarks> Implementacja interfejsu IDisposable </remarks>
        public void Dispose()
        {
            // Nie zmieniaj tego kodu. Umieść kod czyszczący w metodzie „Dispose(bool disposing)”.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc cref="IDataHandler.AddImageInfo"/>
        /// <remarks>
        /// Dodaje dane o obrazie do bazy danych
        /// </remarks>
        public void AddImageInfo(DICOMMetadata metadata, string imageID)
        {
            var patientName = metadata.PatientName;
            var patientID = metadata.PatientID;
            var studyDate = metadata.StudyDate;
            var studyID = metadata.StudyUID;
            try
            {
                if (_conn == null) return;
                _conn.Open();
                SQLDataBaseInsertsHandler.InsertPatient(_conn, patientID, patientName);
                SQLDataBaseInsertsHandler.InsertStudy(_conn, studyID, patientID, studyDate);
                SQLDataBaseInsertsHandler.InsertImage(_conn, imageID, studyID);
                _conn.Close();
            }
            catch (SqliteException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
