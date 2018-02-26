using System.Collections.Generic;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.Configuration;

namespace Gps.Api.Controllers
{
    [Route("api/[controller]")]
    public class GpsController : Controller
    { 
        private readonly GpsDataRetriever _gpsDataRetriever;

        public GpsController(IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionString"];
            _gpsDataRetriever = new GpsDataRetriever(connectionString);
        }
        // GET api/gps
        [HttpGet]
        public string Get()
        {
            return _gpsDataRetriever.GetCurrentCoordinates().ToString();
        }
        
        // GET api/gps
        [HttpGet("{tenantId}")]
        public string Get(int tenantId)
        {
            return _gpsDataRetriever.GetCurrentCoordinates(tenantId).ToString();
        }
    }
}