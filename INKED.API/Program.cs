using Microsoft.EntityFrameworkCore;
using INKED.Infrastructure.Data;
// Marco
using INKED.Application.Interfaces;
using INKED.Application.Services;
using INKED.Infrastructure.Repositories;
using INKED.API.Middleware;
// Jethro
using INKED.Application.ProductCategories;
using INKED.Application.Products;
using INKED.Application.SizeCharts;
using INKED.Application.OrderItems;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddDbContext<INKEDDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("INKEDConnection")));

// Marco: ProductionTeams, Staff, SampleApprovals, OrderProductionAssignments
builder.Services.AddScoped<IProductionTeamRepository, ProductionTeamRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<ISampleApprovalRepository, SampleApprovalRepository>();
builder.Services.AddScoped<IOrderProductionAssignmentRepository, OrderProductionAssignmentRepository>();
builder.Services.AddScoped<IProductionTeamService, ProductionTeamService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<ISampleApprovalService, SampleApprovalService>();
builder.Services.AddScoped<IOrderProductionAssignmentService, OrderProductionAssignmentService>();

// Jethro: ProductCategories, Products, SizeCharts, OrderItems
builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISizeChartRepository, SizeChartRepository>();
builder.Services.AddScoped<ISizeChartService, SizeChartService>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Marco: shared error handling (404/400/409 responses)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();