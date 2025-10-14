using System;
using System.Collections.Generic;
using ASMS.Repositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace ASMS.Repositories.Data;

public partial class VstorageContext : DbContext
{
    public VstorageContext()
    {
    }

    public VstorageContext(DbContextOptions<VstorageContext> options)
        : base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            //optionsBuilder.UseSqlServer("Server=ROG-ZEPHYRUS-G1\\VIETDUC;Database=VStorage;Uid=sa;Pwd=123456;Trusted_Connection=True;TrustServerCertificate=True");
            optionsBuilder.UseSqlServer("Server=LAPTOP-39B7IASC\\SQLEXPRESS;Database=VStorage;Uid=sa;Pwd=1;Trusted_Connection=True;TrustServerCertificate=True");
            
        }
    }

    public virtual DbSet<Building> Buildings { get; set; }

    public virtual DbSet<Container> Containers { get; set; }

    public virtual DbSet<ContainerLocationLog> ContainerLocationLogs { get; set; }

    public virtual DbSet<ContainerType> ContainerTypes { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeRole> EmployeeRoles { get; set; }

    public virtual DbSet<Floor> Floors { get; set; }

    public virtual DbSet<FloorBlock> FloorBlocks { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Shelf> Shelves { get; set; }

    public virtual DbSet<Storage> Storages { get; set; }

    public virtual DbSet<StorageBlock> StorageBlocks { get; set; }

    public virtual DbSet<StorageType> StorageTypes { get; set; }

    public virtual DbSet<TrackingHistory> TrackingHistories { get; set; }

    public virtual DbSet<WorkflowStep> WorkflowSteps { get; set; }

    public virtual DbSet<WorkflowTemplate> WorkflowTemplates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Building>(entity =>
        {
            entity.HasKey(e => e.BuildingCode).HasName("PK__Building__D4DA0325D3A2B9AB");

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
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Container>(entity =>
        {
            entity.HasKey(e => e.ContainerCode).HasName("PK__Containe__874FE471B6792E9D");

            entity.ToTable("Container");

            entity.Property(e => e.ContainerCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FloorCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Height).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Length).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Width).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.FloorCodeNavigation).WithMany(p => p.Containers)
                .HasForeignKey(d => d.FloorCode)
                .HasConstraintName("FK__Container__Floor__656C112C");
        });

        modelBuilder.Entity<ContainerLocationLog>(entity =>
        {
            entity.HasKey(e => e.ContainerLocationLogId).HasName("PK__Containe__06E7FAAA2A79C5C0");

            entity.ToTable("ContainerLocationLog");

            entity.Property(e => e.ContainerLocationLogId)
                .ValueGeneratedNever()
                .HasColumnName("ContainerLocationLogID");
            entity.Property(e => e.Assign)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ContainerCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CurrentFloor)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.OldFloor)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.OrderCode)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.ContainerCodeNavigation).WithMany(p => p.ContainerLocationLogs)
                .HasForeignKey(d => d.ContainerCode)
                .HasConstraintName("FK__Container__Conta__68487DD7");
        });

        modelBuilder.Entity<ContainerType>(entity =>
        {
            entity.HasKey(e => e.ContainerTypeId).HasName("PK__Containe__46FA6FF98608AE3E");

            entity.ToTable("ContainerType");

            entity.Property(e => e.ContainerTypeId)
                .ValueGeneratedNever()
                .HasColumnName("ContainerTypeID");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ProductTypeId).HasColumnName("ProductTypeID");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Volume).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.ProductType).WithMany(p => p.ContainerTypes)
                .HasForeignKey(d => d.ProductTypeId)
                .HasConstraintName("FK__Container__Produ__628FA481");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerCode).HasName("PK__Customer__066785204D979A27");

            entity.ToTable("Customer");

            entity.Property(e => e.CustomerCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeCode).HasName("PK__Employee__1F642549B4A50CB4");

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
            entity.Property(e => e.EmployeeRoleId).HasColumnName("EmployeeRoleID");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
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
                .HasConstraintName("FK__Employee__Buildi__7D439ABD");

            entity.HasOne(d => d.EmployeeRole).WithMany(p => p.Employees)
                .HasForeignKey(d => d.EmployeeRoleId)
                .HasConstraintName("FK__Employee__Employ__7C4F7684");
        });

        modelBuilder.Entity<EmployeeRole>(entity =>
        {
            entity.HasKey(e => e.EmployeeRoleId).HasName("PK__Employee__3461868636646C5A");

            entity.ToTable("EmployeeRole");

            entity.Property(e => e.EmployeeRoleId)
                .ValueGeneratedOnAdd()
                .HasColumnName("EmployeeRoleID");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Floor>(entity =>
        {
            entity.HasKey(e => e.FloorCode).HasName("PK__Floor__394E956B81A832F4");

            entity.ToTable("Floor");

            entity.Property(e => e.FloorCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Height).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Length).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ShelfCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Width).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.ShelfCodeNavigation).WithMany(p => p.Floors)
                .HasForeignKey(d => d.ShelfCode)
                .HasConstraintName("FK__Floor__ShelfCode__5FB337D6");
        });

        modelBuilder.Entity<FloorBlock>(entity =>
        {
            entity.HasKey(e => e.FloorBlockCode).HasName("PK__FloorBlo__DF861D583ABE3DA3");

            entity.ToTable("FloorBlock");

            entity.Property(e => e.FloorBlockCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FloorCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Height).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Length).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Width).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.FloorCodeNavigation).WithMany(p => p.FloorBlocks)
                .HasForeignKey(d => d.FloorCode)
                .HasConstraintName("FK__FloorBloc__Floor__160F4887");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderCode).HasName("PK__Order__999B52287CF5E3A0");

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
                .HasConstraintName("FK__Order__CustomerC__6EF57B66");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D30C9215E8D7");

            entity.ToTable("OrderDetail");

            entity.Property(e => e.OrderDetailId)
                .ValueGeneratedNever()
                .HasColumnName("OrderDetailID");
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.ContainerCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Image)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.OrderCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Quantity).HasMaxLength(500);
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.StorageCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.ContainerCodeNavigation).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ContainerCode)
                .HasConstraintName("FK__OrderDeta__Conta__73BA3083");

            entity.HasOne(d => d.OrderCodeNavigation).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderCode)
                .HasConstraintName("FK__OrderDeta__Order__71D1E811");

            entity.HasOne(d => d.Service).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__OrderDeta__Servi__74AE54BC");

            entity.HasOne(d => d.StorageCodeNavigation).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.StorageCode)
                .HasConstraintName("FK__OrderDeta__Stora__72C60C4A");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.ProductTypeId).HasName("PK__ProductT__A1312F4E69BB1C0D");

            entity.ToTable("ProductType");

            entity.Property(e => e.ProductTypeId)
                .ValueGeneratedNever()
                .HasColumnName("ProductTypeID");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Service__C51BB0EA2D1E364C");

            entity.ToTable("Service");

            entity.Property(e => e.ServiceId)
                .ValueGeneratedNever()
                .HasColumnName("ServiceID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<Shelf>(entity =>
        {
            entity.HasKey(e => e.ShelfCode).HasName("PK__Shelf__24D15D7509E5371F");

            entity.ToTable("Shelf");

            entity.Property(e => e.ShelfCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Height).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Length).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StorageCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Width).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.StorageCodeNavigation).WithMany(p => p.Shelves)
                .HasForeignKey(d => d.StorageCode)
                .HasConstraintName("FK__Shelf__StorageCo__5CD6CB2B");
        });

        modelBuilder.Entity<Storage>(entity =>
        {
            entity.HasKey(e => e.StorageCode).HasName("PK__Storage__AD8F8BC7D5ABE087");

            entity.ToTable("Storage");

            entity.Property(e => e.StorageCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BuildingCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Height).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Length).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ProductTypeId).HasColumnName("ProductTypeID");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StorageTypeId).HasColumnName("StorageTypeID");
            entity.Property(e => e.Width).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.BuildingCodeNavigation).WithMany(p => p.Storages)
                .HasForeignKey(d => d.BuildingCode)
                .HasConstraintName("FK__Storage__Buildin__5535A963");

            entity.HasOne(d => d.ProductType).WithMany(p => p.Storages)
                .HasForeignKey(d => d.ProductTypeId)
                .HasConstraintName("FK__Storage__Product__571DF1D5");

            entity.HasOne(d => d.StorageType).WithMany(p => p.Storages)
                .HasForeignKey(d => d.StorageTypeId)
                .HasConstraintName("FK__Storage__Storage__5629CD9C");
        });

        modelBuilder.Entity<StorageBlock>(entity =>
        {
            entity.HasKey(e => e.StorageBlockCode).HasName("PK__StorageB__10B0C0AD25AD5F9E");

            entity.ToTable("StorageBlock");

            entity.Property(e => e.StorageBlockCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Height).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Length).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StorageCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Width).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.StorageCodeNavigation).WithMany(p => p.StorageBlocks)
                .HasForeignKey(d => d.StorageCode)
                .HasConstraintName("FK__StorageBl__Stora__59FA5E80");
        });

        modelBuilder.Entity<StorageType>(entity =>
        {
            entity.HasKey(e => e.StorageTypeId).HasName("PK__StorageT__C94B8F7DD4F87BA1");

            entity.ToTable("StorageType");

            entity.Property(e => e.StorageTypeId)
                .ValueGeneratedNever()
                .HasColumnName("StorageTypeID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TrackingHistory>(entity =>
        {
            entity.HasKey(e => e.TrackingHistoryId).HasName("PK__Tracking__26DB07A738199CF4");

            entity.ToTable("TrackingHistory");

            entity.Property(e => e.TrackingHistoryId)
                .ValueGeneratedOnAdd()
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
            entity.Property(e => e.OrderDetailCode)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.OrderCodeNavigation).WithMany(p => p.TrackingHistories)
                .HasForeignKey(d => d.OrderCode)
                .HasConstraintName("FK__TrackingH__Order__778AC167");
        });

        modelBuilder.Entity<WorkflowStep>(entity =>
        {
            entity.HasKey(e => e.WorkflowStepId).HasName("PK__Workflow__36121401BCF42ECF");

            entity.ToTable("WorkflowStep");

            entity.Property(e => e.WorkflowStepId)
                .ValueGeneratedOnAdd()
                .HasColumnName("WorkflowStepID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.WorkflowTemplateId).HasColumnName("WorkflowTemplateID");

            entity.HasOne(d => d.WorkflowTemplate).WithMany(p => p.WorkflowSteps)
                .HasForeignKey(d => d.WorkflowTemplateId)
                .HasConstraintName("FK__WorkflowS__Workf__4E88ABD4");
        });

        modelBuilder.Entity<WorkflowTemplate>(entity =>
        {
            entity.HasKey(e => e.WorkflowTemplateId).HasName("PK__Workflow__96E60A37BE8B1C29");

            entity.ToTable("WorkflowTemplate");

            entity.Property(e => e.WorkflowTemplateId)
                .ValueGeneratedNever()
                .HasColumnName("WorkflowTemplateID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StorageTypeId).HasColumnName("StorageTypeID");

            entity.HasOne(d => d.StorageType).WithMany(p => p.WorkflowTemplates)
                .HasForeignKey(d => d.StorageTypeId)
                .HasConstraintName("FK__WorkflowT__Stora__4BAC3F29");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
