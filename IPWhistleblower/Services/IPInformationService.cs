﻿using IPWhistleblower.Helpers;
using IPWhistleblower.Interfaces;

namespace IPWhistleblower.Services
{
    public class IPInformationService : IIPInformationService
    {
        private readonly HttpClient _httpClient;

        public IPInformationService(IHttpClientFactory httpClientFactory)
         {
            _httpClient = httpClientFactory.CreateClient("DefaultClient"); 
        }

        public async Task<IP2CResponse> GetAddressInfoAsync(string ipAddress)
        {
            var url = new Uri($"https://ip2c.org/{ipAddress}"); 
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error getting IP info: {response.StatusCode}");
            
            var responseString = await response.Content.ReadAsStringAsync();
            var ip2cResponse = IP2CResponse.FromResponseString(responseString);

            return ip2cResponse;

        }
    }
}


