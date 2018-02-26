using System;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace GpsUdpReceiver
{
    public class GpsPersister
    {
        private readonly string _connectionString;
        private LogWriter _logWriter;

        public GpsPersister(string connectionString)
        {
            _connectionString = string.IsNullOrEmpty(connectionString) ? "" : connectionString;
            _logWriter = new LogWriter();
        }

        public bool PersistGpsCoordinate(GpsCoordinate gpsCoordinate)
        {
            try
            {
                Task.Run(() =>
                {
                    using (var conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();
                        var comm = conn.CreateCommand();
                        comm.CommandText =
                            "INSERT INTO gps_coordinate_log(tenantid, latitude, longitude, elevation, time_collected) VALUES(@tenantid, @latitude, @longitude, @elevation, @time_collected)";
                        comm.Parameters.AddWithValue("@tenantid", 1);
                        comm.Parameters.AddWithValue("@latitude", gpsCoordinate.Lat);
                        comm.Parameters.AddWithValue("@longitude", gpsCoordinate.Lon);
                        comm.Parameters.AddWithValue("@elevation", gpsCoordinate.Ele);
                        comm.Parameters.AddWithValue("@time_collected", DateTime.Now);
                        comm.ExecuteNonQuery();
                        conn.Close();
                    }
                });
            }
            catch (Exception e)
            {
                PersistError(e, "Error storing GPS coordinate");
                _logWriter.Log(e + "Error storing GPS coordinate");
                
                Console.WriteLine(e);
                return false;
            }

            return true;
        }

        public void PersistError(Exception exception, string errorText, int tenantId = -1)
        {
            Task.Run(() =>
            {
                try
                {
                    using (var conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();
                        var comm = conn.CreateCommand();
                        comm.CommandText =
                            "INSERT INTO gps_eror_log(tenant_id, error_detail, error_text, time_collected) VALUES(@tenant_id, @error_detail, @error_text, @time_collected)";
                        comm.Parameters.AddWithValue("@tenant_id", tenantId);
                        comm.Parameters.AddWithValue("@error_detail", exception);
                        comm.Parameters.AddWithValue("@error_text", errorText);
                        comm.Parameters.AddWithValue("@time_collected", DateTime.Now);
                        comm.ExecuteNonQuery();
                        conn.Close();
                    }
                }
                catch (Exception e)
                {
                    _logWriter.Log(e + "Error storing GPS coordinate");
                    Console.WriteLine(e);
                }
            });
        }
    }
}