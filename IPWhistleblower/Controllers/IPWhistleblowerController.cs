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
        private readonly IIPAddressService _ipAddressService;
        private readonly IIPInformationService _informationService;
        private readonly ApplicationDbContext _context;
        public IPWhistleblowerController(IIPInformationService IPService, ApplicationDbContext context, IIPAddressService ipAddressService)
        {
            _informationService = IPService;
            _context = context;
            _ipAddressService = ipAddressService;
        }

        [HttpGet(Name = "/api/ipinfo/{ipAddress}")]
        public async Task<IActionResult> Get(string ipAddress)
        {
            if (!IPAddress.TryParse(ipAddress, out var _ ))
                return BadRequest(new { Message = "Invalid IP address format." });

            //if (ipAddress in cache)
            //return it;


            var ipAddressFromDb = await _context.IPAddresses.Where(address => address.IP == ipAddress) //1 ii)
                                                            .SingleOrDefaultAsync();  //Think Single here works better instead of First as we should not have double entries
            if (ipAddressFromDb != null)
                return Ok(ipAddressFromDb);
           

            var newAddressInfo = await _informationService.GetAddressInfoAsync(ipAddress); //1 iii)
            if (newAddressInfo != null)
                await _ipAddressService.AddAddressToDbAsync(ipAddress, newAddressInfo);    //a)



            return Ok(newAddressInfo);

        }
    }
}
