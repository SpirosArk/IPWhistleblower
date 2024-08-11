using IPWhistleblower.Helpers;

namespace IPWhistleblower.Interfaces
{
    public interface IIPInformationService
    {
        Task<IEnumerable<IP2CResponse>> GetInformationAsync(string ipAddress);
    }
}
