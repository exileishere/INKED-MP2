namespace INKED.Application.SizeCharts;

/// <summary>
/// Business rules for dbo.SizeCharts, taken from the MP1 script and documentation (Table 8, Section 6.5):
///   FK_SizeCharts_QuotationItems, UQ_SizeCharts_QuotationItemID_Size, CK_SizeCharts_Size (not blank),
///   CK_SizeCharts_Quantity (> 0).
///   Application rule (MP1 6.5): the size quantities of an item add up to QuotationItems.Quantity,
///   so the total of all sizes may never go above the quoted quantity.
///   Application rule (MP1 6.5): an order is locked once the downpayment is paid, so sizes of a
///   quoted item whose order is locked cannot be changed.

/// Errors used by this service (the controller turns them into HTTP codes):
///   ArgumentException         -> 400 Bad Request  (invalid value or missing linked record)
///   InvalidOperationException -> 409 Conflict     (duplicate, record in use, or order locked)
/// </summary>
public class SizeChartService : ISizeChartService
{
    private readonly ISizeChartRepository _repository;

    public SizeChartService(ISizeChartRepository repository)
    {
        _repository = repository;
    }

    public Task<List<SizeChartResponseDto>> GetAllAsync()
        => _repository.GetAllAsync();

    public Task<SizeChartResponseDto?> GetByIdAsync(int id)
        => _repository.GetByIdAsync(id);

    public async Task<SizeChartResponseDto> CreateAsync(CreateSizeChartDto dto)
    {
        var clean = new CreateSizeChartDto
        {
            QuotationItemId = dto.QuotationItemId,
            Size = CleanSize(dto.Size),
            Quantity = ValidQuantity(dto.Quantity)
        };

        var quotedQuantity = await _repository.GetQuotedQuantityAsync(clean.QuotationItemId)
            ?? throw new ArgumentException($"QuotationItemId {clean.QuotationItemId} does not exist in QuotationItems.");

        await EnsureNotLocked(clean.QuotationItemId);

        if (await _repository.SizeExistsForQuotationItemAsync(clean.QuotationItemId, clean.Size))
            throw new InvalidOperationException(
                $"Size '{clean.Size}' already exists for QuotationItemId {clean.QuotationItemId}. Update that row instead.");

        var currentTotal = await _repository.GetTotalSizeQuantityAsync(clean.QuotationItemId);
        EnsureWithinQuotedQuantity(currentTotal + clean.Quantity, quotedQuantity, clean.QuotationItemId);

        return await _repository.AddAsync(clean);
    }

    public async Task<bool> UpdateAsync(int id, UpdateSizeChartDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null) return false;

        var clean = new UpdateSizeChartDto
        {
            Size = CleanSize(dto.Size),
            Quantity = ValidQuantity(dto.Quantity)
        };

        await EnsureNotLocked(existing.QuotationItemId);

        if (await _repository.SizeExistsForQuotationItemAsync(existing.QuotationItemId, clean.Size, id))
            throw new InvalidOperationException(
                $"Size '{clean.Size}' already exists for QuotationItemId {existing.QuotationItemId}.");

        var quotedQuantity = await _repository.GetQuotedQuantityAsync(existing.QuotationItemId)
            ?? throw new ArgumentException($"QuotationItemId {existing.QuotationItemId} no longer exists.");
        var otherSizesTotal = await _repository.GetTotalSizeQuantityAsync(existing.QuotationItemId, id);
        EnsureWithinQuotedQuantity(otherSizesTotal + clean.Quantity, quotedQuantity, existing.QuotationItemId);

        return await _repository.UpdateAsync(id, clean);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null) return false;

        await EnsureNotLocked(existing.QuotationItemId);
        return await _repository.DeleteAsync(id);   // no other table references SizeCharts
    }

    private async Task EnsureNotLocked(int quotationItemId)
    {
        if (await _repository.IsQuotationItemLockedAsync(quotationItemId))
            throw new InvalidOperationException(
                $"The order for QuotationItemId {quotationItemId} is locked (downpayment paid), so its sizes can no longer change.");
    }

    private static void EnsureWithinQuotedQuantity(int newTotal, int quotedQuantity, int quotationItemId)
    {
        if (newTotal > quotedQuantity)
            throw new ArgumentException(
                $"Size quantities for QuotationItemId {quotationItemId} would total {newTotal}, " +
                $"but the quoted quantity is only {quotedQuantity}.");
    }

    private static string CleanSize(string? value)
    {
        var size = (value ?? string.Empty).Trim();
        if (size.Length == 0)
            throw new ArgumentException("Size is required and cannot be blank.");
        if (size.Length > 10)
            throw new ArgumentException("Size must be 10 characters or fewer.");
        return size;
    }

    private static int ValidQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0.");
        return quantity;
    }
}
