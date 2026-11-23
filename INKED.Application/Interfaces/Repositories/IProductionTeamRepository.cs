using INKED.Application.DTOs;

namespace INKED.Application.Interfaces;

/// <summary>
/// Data-access contract. It uses DTOs only, so INKED.Application never depends on the
/// EF entity classes (they live in INKED.Infrastructure.Models). The Response DTO doubles
/// as the already-validated data carrier for writes.
/// </summary>
public interface IProductionTeamRepository
{
    Task<IReadOnlyList<ProductionTeamResponseDto>> GetAllAsync();
    Task<ProductionTeamResponseDto?> GetByIdAsync(int id);
    Task<ProductionTeamResponseDto> AddAsync(ProductionTeamResponseDto team);   // TeamID is ignored
    Task UpdateAsync(ProductionTeamResponseDto team);
    Task DeleteAsync(int id);

    Task<bool> ExistsAsync(int id);
    /// <summary>TeamName is UNIQUE in the database.</summary>
    Task<bool> NameExistsAsync(string teamName, int? excludeId = null);
    /// <summary>True if Staff or FeasibilityChecks still reference the team.</summary>
    Task<bool> HasDependentsAsync(int id);
}
