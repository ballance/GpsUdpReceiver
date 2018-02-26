using System;
using Gps.Api.Models;
using MySql.Data.MySqlClient;

namespace Gps.Api.Controllers
{
    public class GpsDataRetriever
    {
        private readonly string _connectionString;

        public GpsDataRetriever(string connectionString)
        {
            _connectionString = connectionString;
        }

        public GpsCoordinate GetCurrentCoordinates(int tenantId = 1)
        {
            var retrievedCoordinate = new GpsCoordinate();
            using (var conn = new MySqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    var comm = conn.CreateCommand();
                    comm.CommandText =
                        "SELECT latitude, longitude, elevation FROM gps_coordinate_log WHERE tenantid = @tenantId ORDER BY time_collected DESC LIMIT 1";

                    comm.Parameters.AddWithValue("@tenantId", 1);
                    var retrievedCoordinateReader = comm.ExecuteReader();
                    while (retrievedCoordinateReader.Read())
                    {
                        retrievedCoordinate.Lat = Convert.ToDouble(retrievedCoordinateReader.GetString("latitude"));
                        retrievedCoordinate.Lon = Convert.ToDouble(retrievedCoordinateReader.GetString("longitude"));
                        retrievedCoordinate.Ele = Convert.ToDouble(retrievedCoordinateReader.GetString("elevation"));
                    }
                }
                catch (Exception e)
                {
                    // TODO: Write to database
                    Console.WriteLine(e);
                }
                finally
                {
                    conn.Close();
                }

                return retrievedCoordinate;
            }
        }
    }
}