namespace INKED.Application.OrderItems;

/// <summary>
/// Business rules for dbo.OrderItems, taken from the MP1 script and documentation (Table 10, Section 6.5):
///   FK_OrderItems_Orders, FK_OrderItems_Products, CK_OrderItems_Quantity (> 0),
///   CK_OrderItems_UnitPrice (>= 0), LineTotal is a computed column (never typed in).
///   "Quantity and UnitPrice are copied from the accepted quotation" -> the product must be on the
///   order's quotation, and empty Quantity/UnitPrice are copied from that quotation line.
///   Application rule (MP1 6.5): an order is locked once the downpayment is paid
///   (Orders.IsLocked = 1), so its items cannot be added, changed or deleted.

/// Errors used by this service (the controller turns them into HTTP codes):
///   ArgumentException         -> 400 Bad Request  (invalid value or missing linked record)
///   InvalidOperationException -> 409 Conflict     (duplicate, record in use, or order locked)
/// </summary>
public class OrderItemService : IOrderItemService
{
    private readonly IOrderItemRepository _repository;

    public OrderItemService(IOrderItemRepository repository)
    {
        _repository = repository;
    }

    public Task<List<OrderItemResponseDto>> GetAllAsync()
        => _repository.GetAllAsync();

    public Task<OrderItemResponseDto?> GetByIdAsync(int id)
        => _repository.GetByIdAsync(id);

    public async Task<OrderItemResponseDto> CreateAsync(CreateOrderItemDto dto)
    {
        var quotationId = await GetUnlockedOrderQuotationId(dto.OrderId);
        var quotedLines = await GetQuotedLines(quotationId, dto.OrderId, dto.ProductId);

        // Copy from the quotation when the client leaves the value empty
        var clean = new CreateOrderItemDto
        {
            OrderId = dto.OrderId,
            ProductId = dto.ProductId,
            Quantity = ValidQuantity(dto.Quantity ?? SingleLine(quotedLines, dto.ProductId).Quantity),
            UnitPrice = ValidPrice(dto.UnitPrice ?? SingleLine(quotedLines, dto.ProductId).UnitPrice),
            ProductionNotes = CleanNotes(dto.ProductionNotes)
        };

        return await _repository.AddAsync(clean);
    }

    public async Task<bool> UpdateAsync(int id, UpdateOrderItemDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null) return false;

        var clean = new UpdateOrderItemDto
        {
            ProductId = dto.ProductId,
            Quantity = ValidQuantity(dto.Quantity),
            UnitPrice = ValidPrice(dto.UnitPrice),
            ProductionNotes = CleanNotes(dto.ProductionNotes)
        };

        var quotationId = await GetUnlockedOrderQuotationId(existing.OrderId);
        if (clean.ProductId != existing.ProductId)
            await GetQuotedLines(quotationId, existing.OrderId, clean.ProductId);   // new product must also be on the quotation

        return await _repository.UpdateAsync(id, clean);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null) return false;

        await GetUnlockedOrderQuotationId(existing.OrderId);
        return await _repository.DeleteAsync(id);   // no other table references OrderItems
    }

    private async Task<int> GetUnlockedOrderQuotationId(int orderId)
    {
        var quotationId = await _repository.GetOrderQuotationIdAsync(orderId)
            ?? throw new ArgumentException($"OrderId {orderId} does not exist in Orders.");

        if (await _repository.IsOrderLockedAsync(orderId))
            throw new InvalidOperationException(
                $"Order {orderId} is locked (downpayment paid), so its items can no longer be added, changed or deleted.");

        return quotationId;
    }

    private async Task<List<QuotedLineDto>> GetQuotedLines(int quotationId, int orderId, int productId)
    {
        if (!await _repository.ProductExistsAsync(productId))
            throw new ArgumentException($"ProductId {productId} does not exist in Products.");

        var lines = await _repository.GetQuotedLinesAsync(quotationId, productId);
        if (lines.Count == 0)
            throw new ArgumentException(
                $"ProductId {productId} is not on Quotation {quotationId} of Order {orderId}. " +
                "Order items must come from the accepted quotation.");
        return lines;
    }

    private static QuotedLineDto SingleLine(List<QuotedLineDto> lines, int productId)
    {
        if (lines.Count > 1)
            throw new ArgumentException(
                $"ProductId {productId} appears on more than one quotation line. Send Quantity and UnitPrice yourself.");
        return lines[0];
    }

    private static int ValidQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0.");
        return quantity;
    }

    private static decimal ValidPrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException("UnitPrice cannot be negative.");
        if (price > 99999999.99m)
            throw new ArgumentException("UnitPrice is too large for DECIMAL(10,2).");
        if (decimal.Round(price, 2) != price)
            throw new ArgumentException("UnitPrice can only have up to 2 decimal places.");
        return price;
    }

    private static string? CleanNotes(string? value)
    {
        var text = value?.Trim();
        if (string.IsNullOrEmpty(text)) return null;
        if (text.Length > 500)
            throw new ArgumentException("ProductionNotes must be 500 characters or fewer.");
        return text;
    }
}
