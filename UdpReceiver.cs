using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GpsUdpReceiver
{
    public class UdpReceiver
    {
        private static StringBuilder _runningLog;
        private bool _isListening = false;
        private List<GpsCoordinate> _gpsCoordinatesBuffer;
        private GpsPersister _gpsPersister;
        private int _portToListenOn;

        public UdpReceiver(GpsPersister gpsPersister, int portToListenOn)
        {
            _runningLog = new StringBuilder();
            _gpsCoordinatesBuffer = new List<GpsCoordinate>();
            _gpsPersister = gpsPersister;
            _portToListenOn = portToListenOn;
        }

        public string GetCurrentLog()
        {
            return _runningLog.ToString();
        }

        public int GetCurrentLogSize()
        {
            return _runningLog.Length;
        }


        public async Task Listen()
        {
            if (_isListening)
            {
                var errorText = $"Already listening on port {_portToListenOn}.";
                _gpsPersister.PersistError(new HttpListenerException(500), errorText);
                Console.WriteLine(errorText);
                return;
            }

            try
            {
                _isListening = true;


                await Task.Run(() =>
                {
                using (var udpClient = new UdpClient(_portToListenOn))
                {
                    while (true)
                    {
                        var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        var receivedResults = udpClient.Receive(ref remoteEndPoint);

                        var currentLog = Encoding.ASCII.GetString(receivedResults);
                        _runningLog.Append(currentLog);
                        if (currentLog.Length > 0)
                        {
                            ProcessUdpDatagram(currentLog);
                        }
                    }
                }

                });
            }
            catch (Exception e)
            {
                _gpsPersister.PersistError(e, "Error capturing UDP data");
                Console.WriteLine(e);
                _isListening = false;
            }
        }

        // TODO: Find a way to capture tenantId.  Perhaps have a dedicated GPS reciever per client?
        private void ProcessUdpDatagram(string currentLog)
        {
            var splitted = currentLog.Split(',').Select(p => p.Trim()).ToList();

            if (false == splitted.Contains("1"))
            {
                return;
            }
            for (var i = 0; i < splitted.Count; i++)
            {
                if (!splitted[i].Equals("1")) continue;
                
                var gpsCoordinate = new GpsCoordinate();
                try
                {
                    gpsCoordinate.Lat = Convert.ToDouble(splitted[i + 1]);
                    gpsCoordinate.Lon = Convert.ToDouble(splitted[i + 2]);
                    gpsCoordinate.Ele = Convert.ToDouble(splitted[i + 3]);

                    _gpsPersister.PersistGpsCoordinate(gpsCoordinate);
                    
                    Console.WriteLine($"{DateTime.Now}: [{gpsCoordinate}]");
                }
                catch (Exception e)
                {
                    _gpsPersister.PersistError(e, "Failed to capture part of gps coordinate, the parts that succeeded were saved");     
                    Console.WriteLine("Failed to capture part of gps coordinate, the parts that succeeded were saved");
                }
            }
        }
    }
}