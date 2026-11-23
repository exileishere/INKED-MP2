// INKED MP2 | Table: Payments | Layer: Application / Service Interface
// Owner: Rashel Maxine Baquirin (Billing module)
namespace INKED.Application.Payments;

public interface IPaymentService
{
    Task<List<PaymentResponseDto>> GetAllAsync();
    Task<PaymentResponseDto?> GetByIdAsync(int id);
    Task<PaymentResponseDto> CreateAsync(CreatePaymentDto dto);
    Task<bool> UpdateAsync(int id, UpdatePaymentDto dto);
    Task<bool> DeleteAsync(int id);
}
