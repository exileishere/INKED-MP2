// INKED MP2 | Table: Invoices | Layer: Application / Service Interface
// Owner: Rashel Maxine Baquirin (Billing module)
namespace INKED.Application.Invoices;

public interface IInvoiceService
{
    Task<List<InvoiceResponseDto>> GetAllAsync();
    Task<InvoiceResponseDto?> GetByIdAsync(int id);
    Task<InvoiceResponseDto> CreateAsync(CreateInvoiceDto dto);
    Task<bool> UpdateAsync(int id, UpdateInvoiceDto dto);
    Task<bool> DeleteAsync(int id);
}
