using IPWhistleblower.Entities;
using IPWhistleblower.Helpers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace IPWhistleblower.Tests.Services
{
    public class IPAddressServiceTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly SqliteConnection _connection;

        public IPAddressServiceTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:"); //Tried first to use In-Memory Testing, that does not support transactions ended up using sqlite for that reason
            _connection.Open();

            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = new ApplicationDbContext(_dbContextOptions);
            context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _connection.Close();
        }

        [Fact]
        public async Task AddAddressToDbAsync_AddsNewCountryAndIPAddress_WhenCountryDoesNotExist()
        {
            using var context = new ApplicationDbContext(_dbContextOptions);
            var service = new IPAddressService(context);
            var ipAddress = "8.8.8.8";
            var response = new IP2CResponse
            {
                CountryName = "United States of America (the)",
                TwoLetterCode = "US",
                ThreeLetterCode = "USA"
            };

            await service.AddAddressToDbAsync(ipAddress, response);

            var country = await context.Countries.FirstOrDefaultAsync(c => c.TwoLetterCode == response.TwoLetterCode);
            var ipRecord = await context.IPAddresses.FirstOrDefaultAsync(ip => ip.IP == ipAddress);

            Assert.NotNull(country);
            Assert.Equal("United States of America (the)", country.Name);

            Assert.NotNull(ipRecord);
            Assert.Equal(ipAddress, ipRecord.IP);
            Assert.Equal(country.Id, ipRecord.CountryId);
        }

        [Fact]
        public async Task AddAddressToDbAsync_AddsIPAddress_WhenCountryExists()
        {
            using var context = new ApplicationDbContext(_dbContextOptions);
            var existingCountry = new Country
            {
                Name = "United States of America (the)",
                TwoLetterCode = "US",
                ThreeLetterCode = "USA",
                CreatedAt = DateTime.UtcNow
            };
            await context.Countries.AddAsync(existingCountry);
            await context.SaveChangesAsync();

            var service = new IPAddressService(context);
            var ipAddress = "8.8.8.8";
            var response = new IP2CResponse
            {
                CountryName = "United States of America (the)",
                TwoLetterCode = "US",
                ThreeLetterCode = "USA"
            };

            await service.AddAddressToDbAsync(ipAddress, response);

            var ipRecord = await context.IPAddresses.FirstOrDefaultAsync(ip => ip.IP == ipAddress);

            Assert.NotNull(ipRecord);
            Assert.Equal(ipAddress, ipRecord.IP);
            Assert.Equal(existingCountry.Id, ipRecord.CountryId);
        }


        [Fact]
        public async Task AddAddressToDbAsync_RollsBackTransaction_OnError()
        {
            using var context = new ApplicationDbContext(_dbContextOptions);
            var service = new IPAddressService(context);
            var ipAddress = "8.8.8.8";
           

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddAddressToDbAsync(ipAddress, null)); 

            var ipRecord = await context.IPAddresses.FirstOrDefaultAsync(ip => ip.IP == ipAddress);
            Assert.Null(ipRecord); // Ensure nothing was added to the database
        }
    }
}
