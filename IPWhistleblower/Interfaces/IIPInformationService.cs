using IPWhistleblower.Helpers;

namespace IPWhistleblower.Interfaces
{
    public interface IIPInformationService
    {
        Task<IP2CResponse> GetAddressInfoAsync(string ipAddress);
    }
}
