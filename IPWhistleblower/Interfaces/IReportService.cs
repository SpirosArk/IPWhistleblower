using IPWhistleblower.Models;

public interface IReportService
{
    Task<IEnumerable<CountryReport>> GetReportAsync(string[] countryCodes);
}
