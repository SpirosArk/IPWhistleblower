using IPWhistleblower.Helpers;

public interface IIPAddressService
{
    Task AddAddressToDbAsync(string ipAddress, IP2CResponse response);
}