using ASMS.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Data;

public partial class AsmsContext : DbContext
{
    public AsmsContext()
    {
    }

    public AsmsContext(DbContextOptions<AsmsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Box> Boxes { get; set; }

    public virtual DbSet<Building> Buildings { get; set; }

    public virtual DbSet<Cell> Cells { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeRole> EmployeeRoles { get; set; }

    public virtual DbSet<Floor> Floors { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<PaymentHistory> PaymentHistories { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomType> RoomTypes { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<TrackingHistory> TrackingHistories { get; set; }

    public virtual DbSet<WorkflowStep> WorkflowSteps { get; set; }

    public virtual DbSet<WorkflowTemplate> WorkflowTemplates { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { optionsBuilder.UseSqlServer(GetConnectionString()); }

    private string GetConnectionString()
    {
        IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
        return configuration["ConnectionStrings:DefaultConnection"];
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Box>(entity =>
        {
            entity.HasKey(e => e.BoxCode).HasName("PK__Box__CB1C2B234E5BF880");

            entity.ToTable("Box");

            entity.Property(e => e.BoxCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CellCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Height).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Length).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Width).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.CellCodeNavigation).WithMany(p => p.Boxes)
                .HasForeignKey(d => d.CellCode)
                .HasConstraintName("FK__Box__CellCode__619B8048");
        });

        modelBuilder.Entity<Building>(entity =>
        {
            entity.HasKey(e => e.BuildingCode).HasName("PK__Building__D4DA0325478792F2");

            entity.ToTable("Building");

            entity.Property(e => e.BuildingCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Area)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.BuildingName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Cell>(entity =>
        {
            entity.HasKey(e => e.CellCode).HasName("PK__Cell__F6994BB72E832EBC");

            entity.ToTable("Cell");

            entity.Property(e => e.CellCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Height).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Length).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.RoomCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Width).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.RoomCodeNavigation).WithMany(p => p.Cells)
                .HasForeignKey(d => d.RoomCode)
                .HasConstraintName("FK__Cell__RoomCode__59FA5E80");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerCode).HasName("PK__Customer__0667852064317DAB");

            entity.ToTable("Customer");

            entity.Property(e => e.CustomerCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CustomerName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeCode).HasName("PK__Employee__1F6425494C433023");

            entity.ToTable("Employee");

            entity.Property(e => e.EmployeeCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.BuildingCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeRoleId).HasColumnName("EmployeeRoleID");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.BuildingCodeNavigation).WithMany(p => p.Employees)
                .HasForeignKey(d => d.BuildingCode)
                .HasConstraintName("FK__Employee__Buildi__71D1E811");

            entity.HasOne(d => d.EmployeeRole).WithMany(p => p.Employees)
                .HasForeignKey(d => d.EmployeeRoleId)
                .HasConstraintName("FK__Employee__Employ__70DDC3D8");
        });

        modelBuilder.Entity<EmployeeRole>(entity =>
        {
            entity.HasKey(e => e.EmployeeRoleId).HasName("PK__Employee__3461868632E1BC62");

            entity.ToTable("EmployeeRole");

            entity.Property(e => e.EmployeeRoleId)
                .ValueGeneratedNever()
                .HasColumnName("EmployeeRoleID");
            entity.Property(e => e.EmployeeRole1)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("EmployeeRole");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
        });

        modelBuilder.Entity<Floor>(entity =>
        {
            entity.HasKey(e => e.FloorCode).HasName("PK__Floor__394E956B4D721C8D");

            entity.ToTable("Floor");

            entity.Property(e => e.FloorCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BuildingCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.BuildingCodeNavigation).WithMany(p => p.Floors)
                .HasForeignKey(d => d.BuildingCode)
                .HasConstraintName("FK__Floor__BuildingC__534D60F1");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderCode).HasName("PK__Order__999B52280722FFBF");

            entity.ToTable("Order");

            entity.Property(e => e.OrderCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CustomerCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.UnpaidAmount).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.CustomerCodeNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerCode)
                .HasConstraintName("FK__Order__CustomerC__5EBF139D");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D30CF88C1100");

            entity.ToTable("OrderDetail");

            entity.Property(e => e.OrderDetailId)
                .ValueGeneratedNever()
                .HasColumnName("OrderDetailID");
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.BoxCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Image)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.OrderCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Quantity).HasMaxLength(500);
            entity.Property(e => e.RoomCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.BoxCodeNavigation).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.BoxCode)
                .HasConstraintName("FK__OrderDeta__BoxCo__68487DD7");

            entity.HasOne(d => d.OrderCodeNavigation).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderCode)
                .HasConstraintName("FK__OrderDeta__Order__66603565");

            entity.HasOne(d => d.RoomCodeNavigation).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.RoomCode)
                .HasConstraintName("FK__OrderDeta__RoomC__6754599E");

            entity.HasOne(d => d.Service).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__OrderDeta__Servi__693CA210");
        });

        modelBuilder.Entity<PaymentHistory>(entity =>
        {
            entity.HasKey(e => e.PaymentHistoryCode).HasName("PK__PaymentH__F83BEDA4BA9A5F87");

            entity.ToTable("PaymentHistory");

            entity.Property(e => e.PaymentHistoryCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.OrderCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PaymentPlatform)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.OrderCodeNavigation).WithMany(p => p.PaymentHistories)
                .HasForeignKey(d => d.OrderCode)
                .HasConstraintName("FK__PaymentHi__Order__74AE54BC");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomCode).HasName("PK__Room__4F9D5230A600B6D9");

            entity.ToTable("Room");

            entity.Property(e => e.RoomCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FloorCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Height).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Length).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.RoomTypeId).HasColumnName("RoomTypeID");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Width).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.FloorCodeNavigation).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.FloorCode)
                .HasConstraintName("FK__Room__FloorCode__5629CD9C");

            entity.HasOne(d => d.RoomType).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.RoomTypeId)
                .HasConstraintName("FK__Room__RoomTypeID__571DF1D5");
        });

        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.HasKey(e => e.RoomTypeId).HasName("PK__RoomType__BCC896110CC5D09F");

            entity.ToTable("RoomType");

            entity.Property(e => e.RoomTypeId)
                .ValueGeneratedNever()
                .HasColumnName("RoomTypeID");
            entity.Property(e => e.RoomTypeName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Service__C51BB0EAAB265F58");

            entity.ToTable("Service");

            entity.Property(e => e.ServiceId)
                .ValueGeneratedNever()
                .HasColumnName("ServiceID");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TrackingHistory>(entity =>
        {
            entity.HasKey(e => e.TrackingHistoryId).HasName("PK__Tracking__26DB07A7040C89E7");

            entity.ToTable("TrackingHistory");

            entity.Property(e => e.TrackingHistoryId)
                .ValueGeneratedNever()
                .HasColumnName("TrackingHistoryID");
            entity.Property(e => e.ActionType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CurrentAssign)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Image)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.NewStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NextAssign)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.OldStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.OrderCode)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.OrderCodeNavigation).WithMany(p => p.TrackingHistories)
                .HasForeignKey(d => d.OrderCode)
                .HasConstraintName("FK__TrackingH__Order__6C190EBB");
        });

        modelBuilder.Entity<WorkflowStep>(entity =>
        {
            entity.HasKey(e => e.WorkflowStepId).HasName("PK__Workflow__3612140149770D3B");

            entity.ToTable("WorkflowStep");

            entity.Property(e => e.WorkflowStepId)
                .ValueGeneratedNever()
                .HasColumnName("WorkflowStepID");
            entity.Property(e => e.WorkflowTemplateId).HasColumnName("WorkflowTemplateID");

            entity.HasOne(d => d.WorkflowTemplate).WithMany(p => p.WorkflowSteps)
                .HasForeignKey(d => d.WorkflowTemplateId)
                .HasConstraintName("FK__WorkflowS__Workf__4E88ABD4");
        });

        modelBuilder.Entity<WorkflowTemplate>(entity =>
        {
            entity.HasKey(e => e.WorkflowTemplateId).HasName("PK__Workflow__96E60A37CE12083A");

            entity.ToTable("WorkflowTemplate");

            entity.Property(e => e.WorkflowTemplateId)
                .ValueGeneratedNever()
                .HasColumnName("WorkflowTemplateID");
            entity.Property(e => e.RoomTypeId).HasColumnName("RoomTypeID");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TemplateName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.RoomType).WithMany(p => p.WorkflowTemplates)
                .HasForeignKey(d => d.RoomTypeId)
                .HasConstraintName("FK__WorkflowT__RoomT__4BAC3F29");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
