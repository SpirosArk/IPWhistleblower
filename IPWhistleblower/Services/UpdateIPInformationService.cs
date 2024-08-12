using IPWhistleblower.Helpers;
using IPWhistleblower.Services;
using IPWhistleblower.Entities;
using IPWhistleblower.Interfaces;
using Microsoft.EntityFrameworkCore;
using IPWhistleblower.Data;

public class UpdateIPInformationService
{
    private readonly ApplicationDbContext _context;
    private readonly IIPInformationService _ipInformationService;
    private readonly ICacheService _cacheService;
    private const int _batchSize = 100; // Define the batch size for processing

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

                    _cacheService.Remove($"IPAddress_{ipAddress.IP}");
                }
            }
        }
    }

    private async Task<bool> HasIpInfoChangedAsync(IPAddress ipAddress, IP2CResponse latestInfo)
    {
        var currentCountry = await _context.Countries.FindAsync(ipAddress.CountryId);

        if (currentCountry == null)
        {
            return true;
        }

        return currentCountry.TwoLetterCode != latestInfo.TwoLetterCode ||
               currentCountry.ThreeLetterCode != latestInfo.ThreeLetterCode ||
               currentCountry.Name != latestInfo.CountryName;
    }

    private async Task<int> GetCountryIdAsync(IP2CResponse ip2cResponse)
    {
        var country = await _context.Countries
                                     .FirstOrDefaultAsync(c => c.TwoLetterCode == ip2cResponse.TwoLetterCode);

        if (country == null)
        {
            country = new Country
            {
                Name = ip2cResponse.CountryName,
                TwoLetterCode = ip2cResponse.TwoLetterCode,
                ThreeLetterCode = ip2cResponse.ThreeLetterCode,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Countries.AddAsync(country);
            await _context.SaveChangesAsync();
        }

        return country.Id;
    }
}
