using INKED.Application.DTOs;

namespace INKED.Application.Interfaces;

public interface IProductionTeamService
{
    Task<IReadOnlyList<ProductionTeamResponseDto>> GetAllAsync();
    Task<ProductionTeamResponseDto> GetByIdAsync(int id);
    Task<ProductionTeamResponseDto> CreateAsync(CreateProductionTeamDto dto);
    Task<ProductionTeamResponseDto> UpdateAsync(int id, UpdateProductionTeamDto dto);
    Task DeleteAsync(int id);
}
