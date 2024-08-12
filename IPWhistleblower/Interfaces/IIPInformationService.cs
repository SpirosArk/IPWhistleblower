using IPWhistleblower.Helpers;

namespace IPWhistleblower.Interfaces
{
    public interface IIPInformationService
    {
        Task<IP2CResponse> GetInformationAsync(string ipAddress);
    }
}
