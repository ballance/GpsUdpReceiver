using System;
using System.IO;
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
            var connectionString = Configuration["ConnectionString"];
            
            Console.WriteLine($"Starting UDP listener {DateTime.Now}!");

            var currentLogSize = 0;
            var previousLogSize = 0;

            // TODO: Inject this
            var gpsPersister = new GpsPersister(Configuration["ConnectionString"]);
            
            var udpReceiver = new UdpReceiver(gpsPersister);
            

            try
            {
                udpReceiver.Listen();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            while (true)
            {
             //   Console.WriteLine(udpReceiver.GetCurrentLog());
            }
        }
    }

    
}