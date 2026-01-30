using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace DICOMApp.Repository
{

    internal class SQLDataBaseInsertsHandler
    {
        /// <summary>
        /// Dodanie pacjenta jeśli jeszcze nie ma takiego w bazie
        /// </summary>
        /// <param name="conn">Połączenie z bazą danych</param>
        /// <param name="patientID">ID pacjenta</param>
        /// <param name="patientName">Imię i nazwisko pacjenta</param>
        internal static void InsertPatient(SqliteConnection? conn, string patientID, string patientName)
        {
            if (conn == null) return;
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = """
                    INSERT OR IGNORE INTO patients (id, name)
                    VALUES (@pid, @pName)
                    """;

                // parametry
                cmd.Parameters.AddWithValue("@pid", patientID);
                cmd.Parameters.AddWithValue("@pName", patientName);

                cmd.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// Dodanie badania jeśli jeszcze nie ma takiego w bazie
        /// </summary>
        /// <param name="conn">Połączenie z bazą danych</param>
        /// <param name="studyID">ID badania</param>
        /// <param name="patientID">ID pacjenta, którego dotyczy badanie</param>
        /// <param name="studyDate">Data badania</param>
        internal static void InsertStudy(SqliteConnection? conn, string studyID, string patientID, string studyDate)
        {
            if (conn == null) return;
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = """
                    INSERT OR IGNORE INTO studies (id, patient_id, date)
                    VALUES (@sid, @pid, @date)
                    """;

                cmd.Parameters.AddWithValue("@sid", studyID);
                cmd.Parameters.AddWithValue("@pid", patientID);
                cmd.Parameters.AddWithValue("@date", studyDate);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Dodanie obrazu jeśli jeszcze nie ma takiego w bazie
        /// </summary>
        /// <param name="conn">Połączenie z bazą danych</param>
        /// <param name="imageID">ID obrazu</param>
        /// <param name="studyID">ID badania, do którego należy obraz</param>
        internal static void InsertImage(SqliteConnection? conn, string imageID, string studyID)
        {
            if (conn == null) return;
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = """
                    INSERT OR IGNORE INTO images (id, study_id)
                    VALUES (@iid, @sid)
                    """;

                cmd.Parameters.AddWithValue("@iid", imageID);
                cmd.Parameters.AddWithValue("@sid", studyID);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Dodanie informacji o srodku okna powiększającego do obrazu
        /// </summary>
        /// <param name="conn">Połączenie z bazą danych</param>
        /// <param name="imageID">ID obrazu, do którego mają zostać dodane informacje</param>
        /// <param name="x">Srodek okan powiększającego w osi x</param>
        /// <param name="y">Srodek okan powększającego w osi y</param>
        internal static void InsertMagnifiedImageCenterInfo(SqliteConnection? conn, string imageID, double x, double y)
        {
            if (conn == null) return;
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = """
                    INSERT OR IGNORE INTO magnified_image_centers (image_id, center_x, center_y)
                    VALUES (@iid, @x, @y)
                    """;
                // parametry
                cmd.Parameters.AddWithValue("@x", x);
                cmd.Parameters.AddWithValue("@y", y);
                cmd.Parameters.AddWithValue("@iid", imageID);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Dodanie informacji o punktach i długości nerwu wzrokowego do obrazu
        /// </summary>
        /// <param name="conn">Połączenie z bazą danych</param>
        /// <param name="points">Lista punktów (8 wartości), które zostały zaznaczone</param>
        /// <param name="imageID">ID obrazu, do którego mają zostać dodane informacje</param>
        /// <param name="opticNerveLen">Wyznaczona długość nerwu wzrokowego</param>
        internal static void InsertImageInfo(SqliteConnection? conn, List<double> points, string imageID, double opticNerveLen)
        {
            if (conn == null) return;
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = """
                    UPDATE images
                    SET d1x = @d1x, d2x = @d2x, d3x = @d3x, d4x = @d4x,
                        d1y = @d1y, d2y = @d2y, d3y = @d3y, d4y = @d4y,
                        optic_nerve_size = @ons
                    WHERE id = @iid
                    """;
                // parametry
                cmd.Parameters.AddWithValue("@d1x", points[0]);
                cmd.Parameters.AddWithValue("@d2x", points[1]);
                cmd.Parameters.AddWithValue("@d3x", points[2]);
                cmd.Parameters.AddWithValue("@d4x", points[3]);
                cmd.Parameters.AddWithValue("@d1y", points[4]);
                cmd.Parameters.AddWithValue("@d2y", points[5]);
                cmd.Parameters.AddWithValue("@d3y", points[6]);
                cmd.Parameters.AddWithValue("@d4y", points[7]);
                cmd.Parameters.AddWithValue("@ons", opticNerveLen);
                cmd.Parameters.AddWithValue("@iid", imageID);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
