// INKED MP2 | Table: Orders | Layer: Application / Service Implementation
// Owner: Rashel Maxine Baquirin (Billing module)
//
// Business rules (from the MP1 database design, Table 9):
//  1. An order is created only from an ACCEPTED quotation.
//  2. One quotation becomes exactly one order (UQ_Orders_QuotationID).
//  3. RequiredDate is optional, but cannot be earlier than the order date (CK_Orders_RequiredDate).
//  4. OrderDate is set by the system when the order is created.
//  5. Once IsLocked = 1 (60% downpayment paid) the order can no longer be changed, unlocked or deleted.
//  6. An order that still has items, an invoice, payments, etc. cannot be deleted.
using INKED.Application.Billing;

namespace INKED.Application.Orders;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;

    public OrderService(IOrderRepository repository)
    {
        _repository = repository;
    }

    public Task<List<OrderResponseDto>> GetAllAsync() => _repository.GetAllAsync();

    public Task<OrderResponseDto?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

    public async Task<OrderResponseDto> CreateAsync(CreateOrderDto dto)
    {
        var order = new OrderResponseDto
        {
            QuotationId = dto.QuotationId!.Value,
            OrderDate = DateTimeHelper.NowToSecond(),
            RequiredDate = dto.RequiredDate,
            IsLocked = false
        };

        var errors = new ValidationErrors();
        var quotationStatus = await _repository.GetQuotationStatusAsync(order.QuotationId);
        if (quotationStatus is null)
            errors.Add(nameof(order.QuotationId), $"Quotation with ID {order.QuotationId} does not exist.");
        else if (quotationStatus != "Accepted")
            errors.Add(nameof(order.QuotationId),
                $"Only an Accepted quotation can become an order. Quotation {order.QuotationId} is '{quotationStatus}'.");
        ValidateRequiredDate(order, errors);
        errors.ThrowIfAny();

        if (await _repository.QuotationHasOrderAsync(order.QuotationId))
            throw new ConflictException($"Quotation {order.QuotationId} already has an order. One quotation can become only one order.");

        return await _repository.CreateAsync(order);
    }

    public async Task<bool> UpdateAsync(int id, UpdateOrderDto dto)
    {
        var order = await _repository.GetByIdAsync(id);
        if (order is null) return false;

        if (order.IsLocked)
        {
            if (dto.IsLocked == false)
                throw new ConflictException($"Order {id} is locked because the downpayment was paid. It cannot be unlocked.");
            if (dto.RequiredDate != order.RequiredDate)
                throw new ConflictException($"Order {id} is locked because the downpayment was paid. It can no longer be changed.");
            return true; // nothing to change
        }

        order.RequiredDate = dto.RequiredDate;
        order.IsLocked = dto.IsLocked ?? order.IsLocked;

        var errors = new ValidationErrors();
        ValidateRequiredDate(order, errors);
        errors.ThrowIfAny();

        return await _repository.UpdateAsync(order);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var order = await _repository.GetByIdAsync(id);
        if (order is null) return false;

        if (order.IsLocked)
            throw new ConflictException($"Order {id} is locked because the downpayment was paid. It cannot be deleted.");

        var dependents = await _repository.GetDependentRecordTypesAsync(id);
        if (dependents.Count > 0)
            throw new ConflictException(
                $"Cannot delete order {id} because it still has {string.Join(", ", dependents)}. Delete those records first.");

        return await _repository.DeleteAsync(id);
    }

    private static void ValidateRequiredDate(OrderResponseDto order, ValidationErrors errors)
    {
        if (order.RequiredDate.HasValue && order.RequiredDate.Value < DateOnly.FromDateTime(order.OrderDate))
            errors.Add(nameof(order.RequiredDate),
                $"RequiredDate cannot be earlier than the order date ({DateOnly.FromDateTime(order.OrderDate):yyyy-MM-dd}).");
    }
}
