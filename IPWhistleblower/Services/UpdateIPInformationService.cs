using IPWhistleblower.Data;
using IPWhistleblower.Helpers;
using IPWhistleblower.Entities;
using IPWhistleblower.Services;
using IPWhistleblower.Interfaces;
using Microsoft.EntityFrameworkCore;

public class UpdateIPInformationService
{
    private readonly ApplicationDbContext _context;
    private readonly IIPInformationService _ipInformationService;
    private readonly ICacheService _cacheService;
    private const int _batchSize = 100; 

    public UpdateIPInformationService(ApplicationDbContext context, IIPInformationService ipInformationService, ICacheService cacheService)
    {
        _context = context;
        _ipInformationService = ipInformationService;
        _cacheService = cacheService;
    }

    public async Task UpdateIPInformationAsync()
    {
        var ipAddresses = await _context.IPAddresses
                                        .OrderBy(a => a.IP)
                                        .Take(_batchSize)
                                        .ToListAsync();

        foreach (var ipAddress in ipAddresses)
        {
            var latestInfo = await _ipInformationService.GetAddressInfoAsync(ipAddress.IP);

            if (latestInfo != null)
            {
                bool hasChanged = await HasIpInfoChangedAsync(ipAddress, latestInfo);

                if (hasChanged)
                {
                    ipAddress.CountryId = await GetCountryIdAsync(latestInfo);
                    ipAddress.UpdatedAt = DateTime.UtcNow;

                    _context.IPAddresses.Update(ipAddress);
                    await _context.SaveChangesAsync();

                    _cacheService.Remove($"IPAddress_{ipAddress.IP}");  //Invalidate cache if changes were found
                }
            }
        }
    }

    private async Task<bool> HasIpInfoChangedAsync(IPAddress ipAddress, IP2CResponse latestInfo)
    {
        var currentCountry = await _context.Countries.FindAsync(ipAddress.CountryId);

        return currentCountry switch
        {
            null => true,
            var country when
                country.TwoLetterCode != latestInfo.TwoLetterCode ||
                country.ThreeLetterCode != latestInfo.ThreeLetterCode ||
                country.Name != latestInfo.CountryName => true,
            _ => false 
        };
    }

    private async Task<int> GetCountryIdAsync(IP2CResponse ip2cResponse)
    {
        string truncatedCountryName = TruncateString(ip2cResponse.CountryName, 50);


        var country = await _context.Countries
                                     .FirstOrDefaultAsync(c => c.TwoLetterCode == ip2cResponse.TwoLetterCode);

        if (country != null)
            return country.Id;

        country = new Country
        {
            Name = truncatedCountryName,
            TwoLetterCode = ip2cResponse.TwoLetterCode,
            ThreeLetterCode = ip2cResponse.ThreeLetterCode,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Countries.AddAsync(country);
        await _context.SaveChangesAsync();
        return country.Id;
    }


    //This was used because the seeded db has set Name as nvarchar 50 so exceptions were thrown during PeriodicJobServiceExecution
    string TruncateString(string input, int maxLength) =>
    input switch
    {
        null => null,
        _ when input.Length > maxLength => input.Substring(0, maxLength),
        _ => input
    };
}
