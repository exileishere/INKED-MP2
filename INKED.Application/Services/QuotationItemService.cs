using INKED.Application.Helpers;
using INKED.Application.Exceptions;
using INKED.Application.Models.DTO.QuotationItems;
using INKED.Application.Repositories;
using INKED.Application.Interfaces;
using INKED.Domain.Entities.dbo;

namespace INKED.Application.Services;

public class QuotationItemService : IQuotationItemService
{
    private readonly IQuotationItemRepository _repository;

    public QuotationItemService(IQuotationItemRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<QuotationItemResponseDto>> GetAllAsync(CancellationToken ct = default)
        => (await _repository.GetAllAsync(ct)).Select(ToResponse).ToList();

    public async Task<QuotationItemResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        return entity is null ? null : ToResponse(entity);
    }

    public async Task<QuotationItemResponseDto> CreateAsync(CreateQuotationItemDto dto, CancellationToken ct = default)
    {
        if (!await _repository.QuotationExistsAsync(dto.QuotationId, ct))
            throw new BadRequestException($"Quotation {dto.QuotationId} does not exist.");

        if (!await _repository.ProductExistsAsync(dto.ProductId, ct))
            throw new BadRequestException($"Product {dto.ProductId} does not exist.");

        Validate(dto.Quantity, dto.UnitPrice);

        var entity = new QuotationItem
        {
            QuotationId = dto.QuotationId,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            UnitPrice = dto.UnitPrice,
            DesignNotes = ValidationRules.NullIfBlank(dto.DesignNotes)
            // LineTotal is a computed column: never assigned here.
        };

        await _repository.AddAsync(entity, ct);
        return ToResponse(entity);
    }

    public async Task<bool> UpdateAsync(int id, UpdateQuotationItemDto dto, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return false;

        if (!await _repository.ProductExistsAsync(dto.ProductId, ct))
            throw new BadRequestException($"Product {dto.ProductId} does not exist.");

        Validate(dto.Quantity, dto.UnitPrice);

        entity.ProductId = dto.ProductId;
        entity.Quantity = dto.Quantity;
        entity.UnitPrice = dto.UnitPrice;
        entity.DesignNotes = ValidationRules.NullIfBlank(dto.DesignNotes);

        return await _repository.UpdateAsync(entity, ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (await _repository.GetByIdAsync(id, ct) is null) return false;

        if (await _repository.HasSizeChartsAsync(id, ct))
            throw new ConflictException($"Quotation item {id} cannot be deleted because it has size chart rows.");

        return await _repository.DeleteAsync(id, ct);
    }

    // CK_QuotationItems_Quantity (> 0) and CK_QuotationItems_UnitPrice (>= 0, DECIMAL(10,2))
    private static void Validate(int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new BadRequestException("Quantity must be greater than 0.");

        ValidationRules.EnsureMoney(unitPrice, "UnitPrice", precision: 10);
    }

    private static QuotationItemResponseDto ToResponse(QuotationItem e) => new()
    {
        QuotationItemId = e.QuotationItemId,
        QuotationId = e.QuotationId,
        ProductId = e.ProductId,
        Quantity = e.Quantity,
        UnitPrice = e.UnitPrice,
        DesignNotes = e.DesignNotes,
        LineTotal = e.Quantity * e.UnitPrice // same formula as the persisted computed column
    };
}
