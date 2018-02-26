using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace GpsUdpReceiver
{
    class Program
    {
        private static IConfiguration Configuration { get; set; }
        
        static void Main(string[] args)
        {
        
            
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.prod.json");
            
            Configuration = builder.Build();

            var runner = new Runner();
         
            var connectionString = Configuration["ConnectionString"];
            var portToListenOn = Convert.ToInt32(Configuration["UdpPortToListenOn"]);
            var gpsPersister = new GpsPersister(connectionString);
         
            
            Task.Run(async () =>
            {
                await runner.Run(gpsPersister, portToListenOn);
            });
            
            while (true)
            {
//                Console.Write("Press `X` to exit.");
//                var keyEntered = Console.ReadKey();
//                if (keyEntered.Key == ConsoleKey.X)
//                    break;
            }

        }
    }

    public class Runner
    {
        public async Task Run(GpsPersister gpsPersister, int portToListenOn)
        {
            Console.WriteLine($"Starting UDP listener {DateTime.Now}!");

            // TODO: Inject this
            var udpReceiver = new UdpReceiver(gpsPersister, portToListenOn);

            try
            {
                await udpReceiver.Listen();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}