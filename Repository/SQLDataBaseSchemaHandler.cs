using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMApp.Repository
{
    internal class SQLDataBaseSchemaHandler
    {
        internal static void ClearDataBase(SqliteConnection? conn)
        {
            if (conn == null) return;
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = """
                        DROP TABLE IF EXISTS images;
                        DROP TABLE IF EXISTS studies;
                        DROP TABLE IF EXISTS patients;
                        """;
                cmd.ExecuteNonQuery();
            }
        }

        private static void InitializePatientsTable(SqliteConnection conn)
        {

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = """
                        CREATE TABLE IF NOT EXISTS patients (
                            id TEXT PRIMARY KEY,
                            name TEXT NOT NULL
                        );
                        """;
                cmd.ExecuteNonQuery();
            }
        }

        private static void InitializeMagnifiedImageCenterTable(SqliteConnection conn)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = """
                        CREATE TABLE IF NOT EXISTS magnified_image_centers (
                            image_id TEXT PRIMARY KEY,
                            center_x REAL, center_y REAL
                        );
                        """;
                cmd.ExecuteNonQuery();
            }
        }

        private static void InitializeImagesTable(SqliteConnection conn)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = """
                        CREATE TABLE IF NOT EXISTS images (
                            id TEXT PRIMARY KEY, 
                            study_id TEXT NOT NULL,
                            d1x REAL, d2x REAL, d3x REAL, d4x REAL,
                            d1y REAL, d2y REAL, d3y REAL, d4y REAL,
                            optic_nerve_size REAL,
                            FOREIGN KEY (study_id) REFERENCES studies (id) ON DELETE CASCADE
                        );
                        """;
                cmd.ExecuteNonQuery();
            }
        }

        private static void InitializeStudiesTable(SqliteConnection conn)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = """
                        CREATE TABLE IF NOT EXISTS studies (
                            id TEXT PRIMARY KEY,
                            patient_id TEXT NOT NULL,
                            date TEXT NOT NULL,
                            FOREIGN KEY (patient_id) REFERENCES patients (id) ON DELETE CASCADE
                        );
                        """;
                cmd.ExecuteNonQuery();
            }
        }

        internal static void InitializeDatabase(SqliteConnection? conn)
        {
            if (conn == null) return;
            InitializePatientsTable(conn);
            InitializeStudiesTable(conn);
            InitializeImagesTable(conn);
            InitializeMagnifiedImageCenterTable(conn);
        }

    }
}
