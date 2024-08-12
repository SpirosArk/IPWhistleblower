using Dapper;
using System.Text;
using System.Data;
using IPWhistleblower.Models;
using Microsoft.Data.SqlClient;

public class ReportService : IReportService
{
    private readonly string _connectionString;

    public ReportService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<CountryReport>> GetReportAsync(string[] countryCodes)
    {
        var sql = new StringBuilder(@"
            SELECT
                c.Name AS Name,
                COUNT(ip.Id) AS AddressesCount,
                MAX(ip.UpdatedAt) AS LastAddressUpdated
            FROM
                IPAddresses ip
            JOIN
                Countries c ON ip.CountryId = c.Id");

        if (countryCodes != null && countryCodes.Length > 0)
        {
            var formattedCountryCodes = string.Join(", ", countryCodes.Select(c => $"'{c}'"));
            sql.Append($@"
            WHERE
                c.TwoLetterCode IN ({formattedCountryCodes})");
        }

        sql.Append(@"
            GROUP BY
                c.Name");

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            return await connection.QueryAsync<CountryReport>(sql.ToString());
        }
    }
}
