// INKED MP2 | Table: Invoices | Layer: Application / Repository Interface
// Owner: Rashel Maxine Baquirin (Billing module)
namespace INKED.Application.Invoices;

public interface IInvoiceRepository
{
    Task<List<InvoiceResponseDto>> GetAllAsync();
    Task<InvoiceResponseDto?> GetByIdAsync(int id);
    Task<InvoiceResponseDto> CreateAsync(InvoiceResponseDto invoice);
    Task<InvoiceResponseDto?> UpdateAsync(InvoiceResponseDto invoice);
    Task<bool> DeleteAsync(int id);

    // Foreign key and uniqueness checks
    Task<bool> OrderExistsAsync(int orderId);
    Task<bool> OrderHasInvoiceAsync(int orderId);
    Task<bool> InvoiceNoExistsAsync(string invoiceNo, int? excludeInvoiceId = null);
    Task<decimal> GetTotalPaidForOrderAsync(int orderId);
}
