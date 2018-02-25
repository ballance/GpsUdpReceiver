namespace GpsUdpReceiver
{
    public class GpsCoordinate
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double Ele { get; set; }

        public GpsCoordinate()
        {
            Lat = 0;
            Lon = 0;
            Ele = 0;
        }

        public override string ToString()
        {
            return $"{Lat}, {Lon}, {Ele}m";
        }
    }
}