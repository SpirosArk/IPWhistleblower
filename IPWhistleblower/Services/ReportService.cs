using Dapper;
using IPWhistleblower.Models;
using Microsoft.Data.SqlClient; 

public class ReportService : IReportService
{
    private readonly string _connectionString;

    public ReportService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<CountryReport>> GetCountryReportsAsync(string[] countryCodes)
    {
        var countryCodesSql = string.Join("','", countryCodes);
        var sqlQuery = @"
            SELECT
                c.Name AS CountryName,
                COUNT(a.IP) AS AddressesCount,
                MAX(a.UpdatedAt) AS LastAddressUpdated
            FROM
                IPAddresses a
            INNER JOIN
                Countries c ON a.CountryId = c.Id
            GROUP BY
                c.Name
            HAVING
                (c.TwoLetterCode IN (@CountryCodes))";

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var parameters = new { CountryCodes = countryCodesSql };
        return await connection.QueryAsync<CountryReport>(sqlQuery, parameters);
    }
}
