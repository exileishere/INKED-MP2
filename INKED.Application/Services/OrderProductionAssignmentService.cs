using INKED.Application.Common.Exceptions;
using INKED.Application.DTOs;
using INKED.Application.Interfaces;

namespace INKED.Application.Services;

public class OrderProductionAssignmentService : IOrderProductionAssignmentService
{
    // Must match CK_OrderProductionAssignments_Status in the MP1 script.
    private static readonly string[] AllowedStatuses = { "Not Started", "In Progress", "Done", "On Hold" };
    private const string DefaultStatus = "Not Started";

    private readonly IOrderProductionAssignmentRepository _assignments;
    private readonly IStaffRepository _staff;

    public OrderProductionAssignmentService(
        IOrderProductionAssignmentRepository assignments,
        IStaffRepository staff)
    {
        _assignments = assignments;
        _staff = staff;
    }

    public async Task<IReadOnlyList<OrderProductionAssignmentResponseDto>> GetAllAsync()
        => await _assignments.GetAllAsync();

    public async Task<OrderProductionAssignmentResponseDto> GetByIdAsync(int id)
        => await GetOrThrowAsync(id);

    public async Task<OrderProductionAssignmentResponseDto> CreateAsync(CreateOrderProductionAssignmentDto dto)
    {
        // Connects Orders and Staff: both parents must exist.
        if (!await _assignments.OrderExistsAsync(dto.OrderID))
            throw new BadRequestException($"OrderID {dto.OrderID} does not exist in Orders.");

        var staff = await _staff.GetByIdAsync(dto.StaffID)
            ?? throw new BadRequestException($"StaffID {dto.StaffID} does not exist in Staff.");
        if (!staff.IsActive)
            throw new BadRequestException($"Staff member {dto.StaffID} is inactive and cannot be assigned.");

        if (await _assignments.PairExistsAsync(dto.OrderID, dto.StaffID))
            throw new ConflictException(
                $"Staff member {dto.StaffID} is already assigned to order {dto.OrderID}.");

        var status = NormalizeStatus(dto.ProductionStatus ?? DefaultStatus);

        var data = new OrderProductionAssignmentResponseDto
        {
            OrderID = dto.OrderID,
            StaffID = dto.StaffID,
            DateAssigned = DateTime.Now,
            ProductionStatus = status,
            StatusUpdatedAt = status == DefaultStatus ? null : DateTime.Now,
            DueDate = dto.DueDate
        };

        return await _assignments.AddAsync(data);
    }

    public async Task<OrderProductionAssignmentResponseDto> UpdateAsync(
        int id, UpdateOrderProductionAssignmentDto dto)
    {
        var assignment = await GetOrThrowAsync(id);
        var status = NormalizeStatus(dto.ProductionStatus);

        if (!string.Equals(assignment.ProductionStatus, status, StringComparison.Ordinal))
        {
            assignment.ProductionStatus = status;
            assignment.StatusUpdatedAt = DateTime.Now;   // set when the team updates the status
        }
        assignment.DueDate = dto.DueDate;

        await _assignments.UpdateAsync(assignment);
        return assignment;
    }

    public async Task DeleteAsync(int id)
    {
        await GetOrThrowAsync(id);
        await _assignments.DeleteAsync(id);
    }

    // ---- helpers ----
    private async Task<OrderProductionAssignmentResponseDto> GetOrThrowAsync(int id)
        => await _assignments.GetByIdAsync(id)
           ?? throw new NotFoundException($"Production assignment with ID {id} was not found.");

    private static string NormalizeStatus(string? input)
    {
        var match = AllowedStatuses.FirstOrDefault(
            s => s.Equals(input?.Trim(), StringComparison.OrdinalIgnoreCase));
        return match ?? throw new BadRequestException(
            $"ProductionStatus must be one of: {string.Join(", ", AllowedStatuses)}.");
    }
}
