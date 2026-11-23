// INKED MP2 | Table: Payments | Layer: Application / Repository Interface
// Owner: Rashel Maxine Baquirin (Billing module)
using INKED.Application.Invoices;

namespace INKED.Application.Payments;

public interface IPaymentRepository
{
    Task<List<PaymentResponseDto>> GetAllAsync();
    Task<PaymentResponseDto?> GetByIdAsync(int id);

    /// <param name="lockOrder">True for a Downpayment or Full payment: the order's IsLocked flag
    /// is set in the same database save.</param>
    Task<PaymentResponseDto> CreateAsync(PaymentResponseDto payment, bool lockOrder);
    Task<bool> UpdateAsync(PaymentResponseDto payment, bool lockOrder);
    Task<bool> DeleteAsync(int id);

    // Foreign key, uniqueness and payment-rule checks
    Task<bool> OrderExistsAsync(int orderId);
    Task<InvoiceResponseDto?> GetInvoiceForOrderAsync(int orderId);
    Task<bool> PaymentTypeExistsForOrderAsync(int orderId, string paymentType, int? excludePaymentId = null);
    Task<List<string>> GetPaymentTypesForOrderAsync(int orderId, int? excludePaymentId = null);
    Task<decimal> GetTotalPaidForOrderAsync(int orderId, int? excludePaymentId = null);
}
