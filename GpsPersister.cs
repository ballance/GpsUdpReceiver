using System;
using MySql.Data.MySqlClient;

namespace GpsUdpReceiver
{
    public class GpsPersister
    {
        private string _connectionString;

        public GpsPersister(string connectionString)
        {
            _connectionString = string.IsNullOrEmpty(connectionString) ? "" : connectionString;
        }

        public bool PersistGpsCoordinate(GpsCoordinate gpsCoordinate)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    var comm = conn.CreateCommand();
                    comm.CommandText = "INSERT INTO gps_coordinate_log(tenantid, latitude, longitude, elevation, time_collected) VALUES(@tenantid, @latitude, @longitude, @elevation, @time_collected)";
                    comm.Parameters.AddWithValue("@tenantid", 1);
                    comm.Parameters.AddWithValue("@latitude", gpsCoordinate.Lat);
                    comm.Parameters.AddWithValue("@longitude", gpsCoordinate.Lon);
                    comm.Parameters.AddWithValue("@elevation", gpsCoordinate.Ele);
                    comm.Parameters.AddWithValue("@time_collected", DateTime.Now);
                    comm.ExecuteNonQuery();
                    conn.Close();
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }
    }
}