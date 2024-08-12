using IPWhistleblower.Data;
using IPWhistleblower.Entities;
using IPWhistleblower.Helpers;
using Microsoft.EntityFrameworkCore;

public class IPAddressService : IIPAddressService
{
    private readonly ApplicationDbContext _context;

    public IPAddressService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAddressToDbAsync(string ipAddress, IP2CResponse response)
    {
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                var country = await _context.Countries
                                            .FirstOrDefaultAsync(c => c.TwoLetterCode == response.TwoLetterCode);

                if (country == null)
                {
                    country = new Country
                    {
                        Name = response.CountryName,
                        TwoLetterCode = response.TwoLetterCode,
                        ThreeLetterCode = response.ThreeLetterCode,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _context.Countries.AddAsync(country);
                    await _context.SaveChangesAsync(); 
                }

                var ipAddressRecord = new IPAddress
                {
                    CountryId = country.Id,
                    IP = ipAddress,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.IPAddresses.AddAsync(ipAddressRecord);
                await _context.SaveChangesAsync(); 

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw; 
            }
        }
    }
}
