// INKED MP2 | Billing module dependency injection (Orders, Invoices, Payments)
// Owner: Rashel Maxine Baquirin (Billing module)
// Register in Program.cs with ONE line:  builder.Services.AddBillingModule();
using INKED.Application.Invoices;
using INKED.Application.Orders;
using INKED.Application.Payments;
using INKED.Infrastructure.Repositories;

namespace INKED.API.Configurations;

public static class BillingModuleInstaller
{
    public static IServiceCollection AddBillingModule(this IServiceCollection services)
    {
        // Repositories (Infrastructure) - all use the shared INKEDDbContext
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        // Services (Application)
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IPaymentService, PaymentService>();

        return services;
    }
}
