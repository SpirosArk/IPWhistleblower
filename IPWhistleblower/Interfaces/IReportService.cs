using IPWhistleblower.Models;

public interface IReportService
{
    Task<IEnumerable<CountryReport>> GetCountryReportsAsync(string[] countryCodes);
}
