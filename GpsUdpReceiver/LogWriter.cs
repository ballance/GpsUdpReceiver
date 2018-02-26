using System.IO;

namespace GpsUdpReceiver
{
    public class LogWriter
    {
        private StreamWriter _fileStreamWriter;
         

        public LogWriter()
        {
            _fileStreamWriter = new StreamWriter("out.log");
        }

        public void Log(string logmessage)
        {
            _fileStreamWriter.Write(logmessage);
        }

        ~LogWriter()
        {
            _fileStreamWriter.Dispose();
        }
    
    }
}