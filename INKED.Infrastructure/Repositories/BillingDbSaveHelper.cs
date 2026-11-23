// INKED MP2 | Shared helper: translates SQL Server constraint errors into safe API errors
// (UNIQUE -> 409, FOREIGN KEY -> 409, CHECK / NULL / length -> 400). Raw SQL messages are not returned.
// Owner: Rashel Maxine Baquirin (Billing module) - used by Order/Invoice/Payment repositories
using System.Text.RegularExpressions;
using INKED.Application.Billing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace INKED.Infrastructure.Repositories;

public static class BillingDbSaveHelper
{
    public static async Task<int> SaveBillingChangesAsync(this DbContext context)
    {
        try
        {
            return await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sql)
        {
            var constraint = ExtractConstraintName(sql.Message);
            switch (sql.Number)
            {
                case 2601:
                case 2627:
                    throw new ConflictException(
                        $"A record with the same unique value already exists (database constraint {constraint}).");
                case 547 when sql.Message.Contains("CHECK", StringComparison.OrdinalIgnoreCase):
                    throw new ValidationFailedException("Database",
                        $"A value breaks the database rule {constraint}.");
                case 547:
                    throw new ConflictException(
                        $"The operation conflicts with related records (database constraint {constraint}).");
                case 515:
                    throw new ValidationFailedException("Database", "A required value is missing.");
                case 2628:
                case 8152:
                    throw new ValidationFailedException("Database", "A text value is longer than the column allows.");
                default:
                    throw;
            }
        }
    }

    private static string ExtractConstraintName(string message)
    {
        var match = Regex.Match(message, "(?:constraint|index) [\"']([A-Za-z0-9_]+)");
        return match.Success ? match.Groups[1].Value : "unknown";
    }
}
