using System;
using System.Collections.Generic;
using INKED.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace INKED.Infrastructure.Data;

public partial class INKEDDbContext : DbContext
{
    public INKEDDbContext(DbContextOptions<INKEDDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<DeliveryRecord> DeliveryRecords { get; set; }

    public virtual DbSet<FeasibilityCheck> FeasibilityChecks { get; set; }

    public virtual DbSet<Inquiry> Inquiries { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderProductionAssignment> OrderProductionAssignments { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductionTeam> ProductionTeams { get; set; }

    public virtual DbSet<Quotation> Quotations { get; set; }

    public virtual DbSet<QuotationItem> QuotationItems { get; set; }

    public virtual DbSet<SampleApproval> SampleApprovals { get; set; }

    public virtual DbSet<SizeChart> SizeCharts { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    public virtual DbSet<V4> V4s { get; set; }

    public virtual DbSet<V5> V5s { get; set; }

    public virtual DbSet<V6> V6s { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.DateRegistered).HasDefaultValueSql("(CONVERT([date],sysdatetime()))", "DF_Customers_DateRegistered");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FirstName).HasMaxLength(60);
            entity.Property(e => e.LastName).HasMaxLength(60);
            entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");

            entity.HasOne(d => d.Organization).WithMany(p => p.Customers)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("FK_Customers_Organizations");
        });

        modelBuilder.Entity<DeliveryRecord>(entity =>
        {
            entity.HasKey(e => e.DeliveryId);

            entity.Property(e => e.DeliveryId).HasColumnName("DeliveryID");
            entity.Property(e => e.CourierName).HasMaxLength(60);
            entity.Property(e => e.DeliveryAddress).HasMaxLength(250);
            entity.Property(e => e.DeliveryStatus)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValue("Pending", "DF_DeliveryRecords_DeliveryStatus");
            entity.Property(e => e.DeliveryType)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.RecipientName).HasMaxLength(120);
            entity.Property(e => e.TrackingNo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Order).WithMany(p => p.DeliveryRecords)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeliveryRecords_Orders");
        });

        modelBuilder.Entity<FeasibilityCheck>(entity =>
        {
            entity.HasKey(e => e.FeasibilityId);

            entity.Property(e => e.FeasibilityId).HasColumnName("FeasibilityID");
            entity.Property(e => e.InquiryId).HasColumnName("InquiryID");
            entity.Property(e => e.Remarks).HasMaxLength(300);
            entity.Property(e => e.RequestedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_FeasibilityChecks_RequestedAt");
            entity.Property(e => e.RespondedAt).HasPrecision(0);
            entity.Property(e => e.Result)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValue("Pending", "DF_FeasibilityChecks_Result");
            entity.Property(e => e.TeamId).HasColumnName("TeamID");

            entity.HasOne(d => d.Inquiry).WithMany(p => p.FeasibilityChecks)
                .HasForeignKey(d => d.InquiryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FeasibilityChecks_Inquiries");

            entity.HasOne(d => d.Team).WithMany(p => p.FeasibilityChecks)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FeasibilityChecks_ProductionTeams");
        });

        modelBuilder.Entity<Inquiry>(entity =>
        {
            entity.Property(e => e.InquiryId).HasColumnName("InquiryID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.DateReceived)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_Inquiries_DateReceived");
            entity.Property(e => e.FirstResponseAt).HasPrecision(0);
            entity.Property(e => e.HandledByUserId).HasColumnName("HandledByUserID");
            entity.Property(e => e.InquiryChannel)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("Inquiry Form", "DF_Inquiries_InquiryChannel");
            entity.Property(e => e.InquiryDetails).HasMaxLength(1000);
            entity.Property(e => e.Outcome)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Open", "DF_Inquiries_Outcome");

            entity.HasOne(d => d.Customer).WithMany(p => p.Inquiries)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inquiries_Customers");

            entity.HasOne(d => d.HandledByUser).WithMany(p => p.Inquiries)
                .HasForeignKey(d => d.HandledByUserId)
                .HasConstraintName("FK_Inquiries_UserAccounts");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasIndex(e => e.InvoiceNo, "UQ_Invoices_InvoiceNo").IsUnique();

            entity.HasIndex(e => e.OrderId, "UQ_Invoices_OrderID").IsUnique();

            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");
            entity.Property(e => e.ConfirmedAt).HasPrecision(0);
            entity.Property(e => e.DownpaymentAmount)
                .HasComputedColumnSql("(CONVERT([decimal](10,2),[TotalAmount]*[DownpaymentRate]))", true)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.DownpaymentRate)
                .HasDefaultValue(0.60m, "DF_Invoices_DownpaymentRate")
                .HasColumnType("decimal(3, 2)");
            entity.Property(e => e.InvoiceDate).HasDefaultValueSql("(CONVERT([date],sysdatetime()))", "DF_Invoices_InvoiceDate");
            entity.Property(e => e.InvoiceNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValue("Sent", "DF_Invoices_Status");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Order).WithOne(p => p.Invoice)
                .HasForeignKey<Invoice>(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoices_Orders");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasIndex(e => e.QuotationId, "UQ_Orders_QuotationID").IsUnique();

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.OrderDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_Orders_OrderDate");
            entity.Property(e => e.QuotationId).HasColumnName("QuotationID");

            entity.HasOne(d => d.Quotation).WithOne(p => p.Order)
                .HasForeignKey<Order>(d => d.QuotationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Quotations");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.Property(e => e.OrderItemId).HasColumnName("OrderItemID");
            entity.Property(e => e.LineTotal)
                .HasComputedColumnSql("([Quantity]*[UnitPrice])", true)
                .HasColumnType("decimal(21, 2)");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.ProductionNotes)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItems_Orders");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItems_Products");
        });

        modelBuilder.Entity<OrderProductionAssignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId);

            entity.HasIndex(e => new { e.OrderId, e.StaffId }, "UQ_OrderProductionAssignments_OrderID_StaffID").IsUnique();

            entity.Property(e => e.AssignmentId).HasColumnName("AssignmentID");
            entity.Property(e => e.DateAssigned)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_OrderProductionAssignments_DateAssigned");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductionStatus)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValue("Not Started", "DF_OrderProductionAssignments_ProductionStatus");
            entity.Property(e => e.StaffId).HasColumnName("StaffID");
            entity.Property(e => e.StatusUpdatedAt).HasPrecision(0);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderProductionAssignments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderProductionAssignments_Orders");

            entity.HasOne(d => d.Staff).WithMany(p => p.OrderProductionAssignments)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderProductionAssignments_Staff");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId);

            entity.HasIndex(e => e.StageNo, "UQ_OrderStatuses_StageNo").IsUnique();

            entity.HasIndex(e => e.StatusName, "UQ_OrderStatuses_StatusName").IsUnique();

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.StatusName)
                .HasMaxLength(40)
                .IsUnicode(false);
        });

        modelBuilder.Entity<OrderStatusHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId);

            entity.ToTable("OrderStatusHistory");

            entity.Property(e => e.HistoryId).HasColumnName("HistoryID");
            entity.Property(e => e.ChangedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_OrderStatusHistory_ChangedAt");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.Remarks).HasMaxLength(200);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderStatusHistories)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderStatusHistory_Orders");

            entity.HasOne(d => d.Status).WithMany(p => p.OrderStatusHistories)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderStatusHistory_OrderStatuses");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");
            entity.Property(e => e.OrganizationName).HasMaxLength(150);
            entity.Property(e => e.OrganizationType)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasIndex(e => new { e.OrderId, e.PaymentType }, "UQ_Payments_OrderID_PaymentType").IsUnique();

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.PaymentDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_Payments_PaymentDate");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PaymentType)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.ReferenceNo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_Orders");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(e => e.ProductName, "UQ_Products_ProductName").IsUnique();

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.BasePrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Products_IsActive");
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_ProductCategories");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId);

            entity.HasIndex(e => e.CategoryName, "UQ_ProductCategories_CategoryName").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ProductionType)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ProductionTeam>(entity =>
        {
            entity.HasKey(e => e.TeamId);

            entity.HasIndex(e => e.TeamName, "UQ_ProductionTeams_TeamName").IsUnique();

            entity.Property(e => e.TeamId).HasColumnName("TeamID");
            entity.Property(e => e.TeamName)
                .HasMaxLength(40)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Quotation>(entity =>
        {
            entity.Property(e => e.QuotationId).HasColumnName("QuotationID");
            entity.Property(e => e.DateIssued)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_Quotations_DateIssued");
            entity.Property(e => e.InquiryId).HasColumnName("InquiryID");
            entity.Property(e => e.PreparedByUserId).HasColumnName("PreparedByUserID");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValue("Draft", "DF_Quotations_Status");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.Inquiry).WithMany(p => p.Quotations)
                .HasForeignKey(d => d.InquiryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Quotations_Inquiries");

            entity.HasOne(d => d.PreparedByUser).WithMany(p => p.Quotations)
                .HasForeignKey(d => d.PreparedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Quotations_UserAccounts");
        });

        modelBuilder.Entity<QuotationItem>(entity =>
        {
            entity.Property(e => e.QuotationItemId).HasColumnName("QuotationItemID");
            entity.Property(e => e.DesignNotes)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.LineTotal)
                .HasComputedColumnSql("([Quantity]*[UnitPrice])", true)
                .HasColumnType("decimal(21, 2)");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.QuotationId).HasColumnName("QuotationID");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Product).WithMany(p => p.QuotationItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QuotationItems_Products");

            entity.HasOne(d => d.Quotation).WithMany(p => p.QuotationItems)
                .HasForeignKey(d => d.QuotationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QuotationItems_Quotations");
        });

        modelBuilder.Entity<SampleApproval>(entity =>
        {
            entity.HasIndex(e => new { e.OrderId, e.RevisionNo }, "UQ_SampleApprovals_OrderID_RevisionNo").IsUnique();

            entity.Property(e => e.SampleApprovalId).HasColumnName("SampleApprovalID");
            entity.Property(e => e.ApprovalStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending", "DF_SampleApprovals_ApprovalStatus");
            entity.Property(e => e.CustomerRemarks).HasMaxLength(500);
            entity.Property(e => e.DateResponded).HasPrecision(0);
            entity.Property(e => e.DateSent)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_SampleApprovals_DateSent");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.RevisionNo).HasDefaultValue((byte)1, "DF_SampleApprovals_RevisionNo");
            entity.Property(e => e.SamplePhotoPath).HasMaxLength(260);

            entity.HasOne(d => d.Order).WithMany(p => p.SampleApprovals)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SampleApprovals_Orders");
        });

        modelBuilder.Entity<SizeChart>(entity =>
        {
            entity.HasIndex(e => new { e.QuotationItemId, e.Size }, "UQ_SizeCharts_QuotationItemID_Size").IsUnique();

            entity.Property(e => e.SizeChartId).HasColumnName("SizeChartID");
            entity.Property(e => e.QuotationItemId).HasColumnName("QuotationItemID");
            entity.Property(e => e.Size)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.QuotationItem).WithMany(p => p.SizeCharts)
                .HasForeignKey(d => d.QuotationItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SizeCharts_QuotationItems");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.Property(e => e.StaffId).HasColumnName("StaffID");
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.FirstName).HasMaxLength(60);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Staff_IsActive");
            entity.Property(e => e.LastName).HasMaxLength(60);
            entity.Property(e => e.TeamId).HasColumnName("TeamID");

            entity.HasOne(d => d.Team).WithMany(p => p.Staff)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Staff_ProductionTeams");
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.HasIndex(e => e.Username, "UQ_UserAccounts_Username").IsUnique();

            entity.HasIndex(e => e.CustomerId, "UX_UserAccounts_CustomerID")
                .IsUnique()
                .HasFilter("([CustomerID] IS NOT NULL)");

            entity.HasIndex(e => e.StaffId, "UX_UserAccounts_StaffID")
                .IsUnique()
                .HasFilter("([StaffID] IS NOT NULL)");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_UserAccounts_CreatedAt");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_UserAccounts_IsActive");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.StaffId).HasColumnName("StaffID");
            entity.Property(e => e.UserRole)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Customer).WithOne(p => p.UserAccount)
                .HasForeignKey<UserAccount>(d => d.CustomerId)
                .HasConstraintName("FK_UserAccounts_Customers");

            entity.HasOne(d => d.Staff).WithOne(p => p.UserAccount)
                .HasForeignKey<UserAccount>(d => d.StaffId)
                .HasConstraintName("FK_UserAccounts_Staff");
        });

        modelBuilder.Entity<V4>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V4");

            entity.Property(e => e.ConstraintName).HasMaxLength(128);
            entity.Property(e => e.Kind)
                .HasMaxLength(7)
                .IsUnicode(false);
            entity.Property(e => e.TableName).HasMaxLength(128);
        });

        modelBuilder.Entity<V5>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V5");

            entity.Property(e => e.ConstraintName).HasMaxLength(128);
            entity.Property(e => e.Kind)
                .HasMaxLength(17)
                .IsUnicode(false);
            entity.Property(e => e.TableName).HasMaxLength(128);
        });

        modelBuilder.Entity<V6>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V6");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
