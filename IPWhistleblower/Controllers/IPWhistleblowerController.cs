using System.Net;
using IPWhistleblower.Helpers;
using Microsoft.AspNetCore.Mvc;
using IPWhistleblower.Services;
using IPWhistleblower.Interfaces;
using Microsoft.EntityFrameworkCore;
using IPWhistleblower.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using IPWhistleblower.Models;
using Microsoft.IdentityModel.Tokens;
using System.Data.Common;

namespace IPWhistleblower.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IPWhistleblowerController : ControllerBase
    {
        private readonly IIPAddressService _ipAddressService;
        private readonly IIPInformationService _informationService;
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cacheService;
        private readonly IReportService _reportService;

        public IPWhistleblowerController(IIPInformationService IPService, 
                                         ApplicationDbContext context, 
                                         IIPAddressService ipAddressService, 
                                         ICacheService cacheService,
                                         IReportService reportService)
        {
            _informationService = IPService;
            _context = context;
            _ipAddressService = ipAddressService;
            _cacheService = cacheService;
            _reportService = reportService;
        }


        [HttpGet(Name = "/api/ipinfo/{ipAddress}")]
        public async Task<IActionResult> RetrieveIPInfo(string ipAddress)
        {
            if (!IPAddress.TryParse(ipAddress, out var _ ))                                             //Validation for quicker response
                return BadRequest(new { Message = "Invalid IP address format." });

            var cachedIP = _cacheService.Get<IP2CResponse>($"IPAddress_{ipAddress}");                   //1st search Cache with the selected key

            if (cachedIP != null)
                return Ok(cachedIP);


            var ipAddressFromDb = await _context.IPAddresses.Where(address => address.IP == ipAddress)  //2nd search in Database
                                                            .SingleOrDefaultAsync();                    //think Single here works better instead of First as we should not have double entries
            if (ipAddressFromDb != null)
                return Ok(ipAddressFromDb);


            var newAddressInfo = await _informationService.GetAddressInfoAsync(ipAddress);              //3rd do an API call 
            if (newAddressInfo != null)
            {
                await _ipAddressService.AddAddressToDbAsync(ipAddress, newAddressInfo);                 //save in Database
                _cacheService.Set($"IPAddress_{ipAddress}",
                                  newAddressInfo);                                           
            }
            return Ok(newAddressInfo);

        }

        [HttpGet("report")]
        public async Task<IActionResult> GetReport([FromQuery] string[] countryCodes)
        {
            var report = await _reportService.GetReportAsync(countryCodes);
            return Ok(report);
        }
    }
}
