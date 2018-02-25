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

        public UdpReceiver(GpsPersister gpsPersister)
        {
            _runningLog = new StringBuilder();
            _gpsCoordinatesBuffer = new List<GpsCoordinate>();
            _gpsPersister = gpsPersister;
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
                throw new ApplicationException("Already listening.");
                return;
            }

            try
            {
                _isListening = true;

//                await Task.Run(async () =>
//                {
//                    while (true)
//                    {
//                        if (_runningLog.Length <= 0)
//                        {
//                            Console.WriteLine("No data received from GPS device...");
//                        }
//                        else
//                        {
//                            Console.WriteLine("We can haz GPS data from GPS device...");
////                            Console.WriteLine(_runningLog);
//                        }
//
//                        await Task.Delay(1000);
//                    }
//                });

//                Task.Run(() =>
//                {
                using (var udpClient = new UdpClient(10101))
                {
                    while (true)
                    {
                        var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        var receivedResults = udpClient.Receive(ref remoteEndPoint);

                        var _currentLog = Encoding.ASCII.GetString(receivedResults);
                        _runningLog.Append(_currentLog);
                        if (_currentLog.Length > 0)
                        {
                            ProcessUDPDatagram(_currentLog);
                            //Console.WriteLine($"len: {_currentLog.Length}");
                            //Console.WriteLine(_currentLog);
                        }

                        //await Task.Delay(1000);
                    }
                }

                //});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // TODO: Better error handling here
                _isListening = false;
            }
        }

        private void ProcessUDPDatagram(string currentLog)
        {
            var gpsCoordinateList = new List<GpsCoordinate>();
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
                    Console.WriteLine("Failed to capture part of gps coordinate, the parts that succeeded were saved");
                }
            }
        }
    }
}