using IPWhistleblower.Helpers;
using IPWhistleblower.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace IPWhistleblower.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IPWhistleblowerController : ControllerBase
    {
        private readonly IIPInformationService _informationService;
        private readonly ApplicationDbContext _context;
        public IPWhistleblowerController(IIPInformationService IPService, ApplicationDbContext context)
        {
            _informationService = IPService;
            _context = context;
        }

        [HttpGet(Name = "/api/ipinfo/{ipAddress}")]
        public async Task<IActionResult> Get(string ipAddress)
        {
            if (!IPAddress.TryParse(ipAddress, out var _ ))
            {
                return BadRequest(new { Message = "Invalid IP address format." });
            }

            //if (ipAddress in cache)
            //return it;


            var ipAddressFromDb = await _context.IPAddresses.Where(address => address.IP == ipAddress)
                                                            .FirstOrDefaultAsync();

            if (ipAddressFromDb != null)
                return Ok(ipAddressFromDb);
           
            var response = await _informationService.GetInformationAsync(ipAddress);


            return Ok(response);

        }
    }
}
