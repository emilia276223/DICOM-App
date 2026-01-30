using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace DICOMApp.Repository
{
    /// <summary>
    /// Klasa obsługująca połączenie z bazą danych SQLite
    /// </summary>
    /// <remarks>
    /// Implementuje wzorzec IDisposable do prawidłowego zarządzania zasobami
    /// </remarks>
    public class SQLDataBaseConnectionHandler : IDisposable
    {
        private string _dirPath, _dbFilePath;
        private  SqliteConnection? _sqlConnection;
        private bool disposedValue;

        /// <summary>
        /// Tworzy handler połączenia z bazą danych SQLite
        /// </summary>
        /// <param name="dirPath">Nazwa katalogu, w którym jest zapisany plik bazy danych</param>
        /// <param name="dbFilePath">Pełna ścieżka do pliku bazy danych SQLite</param>
        public SQLDataBaseConnectionHandler(string dirPath, string dbFilePath)
        {
            _dirPath = dirPath;
            _dbFilePath = dbFilePath;
        }

        /// <summary>
        /// Otwiera połączenie z bazą danych SQLite
        /// </summary>
        /// <remarks>
        /// Jeśli jest jakiś problem z otwieraniem pliku z bazą danych następuje
        /// skopiowanie aktualnego pliku bazy w inne miejsce i stworzenie nowego pliku
        /// bazy danych w tym miejscu
        /// </remarks>
        /// <returns>Zwraca otwarte połączenie SqliteConnection</returns>
        /// <exception cref="Exception">Wyrzucany w przypadku problemów z bazą danych</exception>"
        public SqliteConnection OpenConnection()
        {
            var connString = new SqliteConnectionStringBuilder
            {
                DataSource = _dbFilePath,
                Mode = SqliteOpenMode.ReadWriteCreate,
                Cache = SqliteCacheMode.Shared
            }.ToString();

            _sqlConnection = new SqliteConnection(connString);

            try
            {
                _sqlConnection.Open();
                return _sqlConnection;
            }
            catch (SqliteException ex)
            {
                // zamknięcie połączenia jeśli otwarte i się da
                try { _sqlConnection.Close(); 
                    _sqlConnection.Dispose();
                } catch { }


                // jeśli plik nie jest bazą danych
                if (ex.SqliteErrorCode == 26)
                {
                    // zapisanie tego co jest gdzies indziej i stworzenie nowego poprawnego pliku
                    var corruptedFilePath = Path.Combine(
                        _dirPath, 
                        "_sql_corrupted_database" + 
                        DateTime.Now.Ticks);
                    File.Copy(_dbFilePath, corruptedFilePath);
                    File.Delete(_dbFilePath);
                    try
                    {

                        _sqlConnection = new SqliteConnection(connString);
                        _sqlConnection.Open();
                        return _sqlConnection;
                    }
                    catch
                    {
                        throw new Exception("Database problem");
                    }

                } 
                
                // jeśli cos innego
                else 
                {
                    throw new Exception("Database problem");
                }

            }
        }

        /// <summary>
        /// Zamyka połączenie z bazą danych SQLite i zwalnia zasoby
        /// </summary>
        /// <param name="disposing">Indykator czy metoda jest wywoływana z Dispose</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_sqlConnection != null && _sqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        _sqlConnection.Close();
                        _sqlConnection.Dispose();
                        disposedValue = true;
                    }
                }
                disposedValue = true;
            }
        }

        /// <summary>
        /// Zwalnia zasoby zarządzane przez klasę i zamyka połączenie z bazą danych
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
