using IPWhistleblower.Services;

namespace IPWhistleblower.Tests.Services
{
    public class IPInformationServiceTests
    {
        private readonly IPInformationService _service;

        public IPInformationServiceTests()
        {
            var httpClient = new HttpClient(); 
            _service = new IPInformationService(new DefaultHttpClientFactory(httpClient));
        }

        [Fact]
        public async Task GetAddressInfoAsync_ReturnsIP2CResponse_ForValidIP()
        {
            var result = await _service.GetAddressInfoAsync("8.8.8.8");

            Assert.NotNull(result);
            Assert.Equal("US", result.TwoLetterCode);
            Assert.Equal("USA", result.ThreeLetterCode);
            Assert.Equal("United States of America (the)", result.CountryName);
        }

        [Fact]
        public async Task GetAddressInfoAsync_ThrowsException_ForInvalidIP()
        {
            await Assert.ThrowsAsync<Exception>(() => _service.GetAddressInfoAsync("invalid-ip"));
        }
    }

    public class DefaultHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpClient _httpClient;

        public DefaultHttpClientFactory(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public HttpClient CreateClient(string name)
        {
            return _httpClient;
        }
    }
}
