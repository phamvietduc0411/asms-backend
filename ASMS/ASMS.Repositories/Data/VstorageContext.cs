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

    public virtual DbSet<Building> Buildings { get; set; }

    public virtual DbSet<Container> Containers { get; set; }

    public virtual DbSet<ContainerLocationLog> ContainerLocationLogs { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeRole> EmployeeRoles { get; set; }

    public virtual DbSet<Floor> Floors { get; set; }

    public virtual DbSet<FloorBlock> FloorBlocks { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<PaymentHistory> PaymentHistories { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Shelf> Shelves { get; set; }

    public virtual DbSet<Storage> Storages { get; set; }

    public virtual DbSet<StorageBlock> StorageBlocks { get; set; }

    public virtual DbSet<StorageType> StorageTypes { get; set; }

    public virtual DbSet<TrackingHistory> TrackingHistories { get; set; }

    public virtual DbSet<WorkflowStep> WorkflowSteps { get; set; }

    public virtual DbSet<WorkflowTemplate> WorkflowTemplates { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-39B7IASC\\SQLEXPRESS;Database=VStoragePublic;Uid=sa;Pwd=1;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Building>(entity =>
        {
            entity.ToTable("Building");

            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Area)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.BuildingCode)
                .HasMaxLength(50)
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

            entity.HasIndex(e => e.FloorCode, "IX_Container_FloorCode");

            entity.Property(e => e.ContainerCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CurrentItemCount).HasDefaultValue(0);
            entity.Property(e => e.CurrentWeight)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.FloorCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.HasFragileItems).HasDefaultValue(false);
            entity.Property(e => e.HasHeavyItems).HasDefaultValue(false);
            entity.Property(e => e.Height).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.LastOptimizedDate).HasColumnType("datetime");
            entity.Property(e => e.Length).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MaxItems).HasDefaultValue(50);
            entity.Property(e => e.MaxWeight)
                .HasDefaultValue(100m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.OptimizationScore)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.PositionX).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PositionY).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PositionZ).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ProductTypeId).HasColumnName("ProductTypeID");
            entity.Property(e => e.RotationAngle).HasDefaultValue(0);
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TotalVolume)
                .HasComputedColumnSql("(([Length]*[Width])*[Height])", false)
                .HasColumnType("decimal(32, 6)");
            entity.Property(e => e.UsedVolume)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(15, 2)");
            entity.Property(e => e.UtilizationRate)
                .HasComputedColumnSql("(case when ([Length]*[Width])*[Height]>(0) then ([UsedVolume]/(([Length]*[Width])*[Height]))*(100) else (0) end)", false)
                .HasColumnType("decimal(38, 15)");
            entity.Property(e => e.Width).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.FloorCodeNavigation).WithMany(p => p.Containers)
                .HasForeignKey(d => d.FloorCode)
                .HasConstraintName("FK__Container__Floor__656C112C");

            entity.HasOne(d => d.ProductType).WithMany(p => p.Containers)
                .HasForeignKey(d => d.ProductTypeId)
                .HasConstraintName("FK_Container_ProductType");
        });

        modelBuilder.Entity<ContainerLocationLog>(entity =>
        {
            entity.HasKey(e => e.ContainerLocationLogId).HasName("PK__Containe__06E7FAAA2A79C5C0");

            entity.ToTable("ContainerLocationLog");

            entity.HasIndex(e => e.ContainerCode, "IX_ContainerLocationLog_ContainerCode");

            entity.HasIndex(e => new { e.ContainerCode, e.UpdatedDate }, "IX_ContainerLocationLog_Container_Date");

            entity.HasIndex(e => e.OrderCode, "IX_ContainerLocationLog_Order");

            entity.Property(e => e.ContainerLocationLogId)
                .ValueGeneratedNever()
                .HasColumnName("ContainerLocationLogID");
            entity.Property(e => e.Algorithm)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ContainerCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CurrentFloor)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.OldFloor)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.OrderCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PerformedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Reason).HasMaxLength(500);

            entity.HasOne(d => d.ContainerCodeNavigation).WithMany(p => p.ContainerLocationLogs)
                .HasForeignKey(d => d.ContainerCode)
                .HasConstraintName("FK__Container__Conta__68487DD7");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customer");

            entity.HasIndex(e => e.CustomerCode, "AK_Customer_CustomerCode").IsUnique();

            entity.HasIndex(e => e.CustomerCode, "UQ_Customer_CustomerCode").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CustomerCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Password)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__1F642549B4A50CB4");

            entity.HasIndex(e => e.BuildingId, "IX_Employees_BuildingId");

            entity.HasIndex(e => e.EmployeeRoleId, "IX_Employees_EmployeeRoleID");

            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeRoleId).HasColumnName("EmployeeRoleID");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
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

            entity.HasOne(d => d.Building).WithMany(p => p.Employees)
                .HasForeignKey(d => d.BuildingId)
                .HasConstraintName("FK__Employee__Buildi__7D439ABD");

            entity.HasOne(d => d.EmployeeRole).WithMany(p => p.Employees)
                .HasForeignKey(d => d.EmployeeRoleId)
                .HasConstraintName("FK__Employee__Employ__7C4F7684");
        });

        modelBuilder.Entity<EmployeeRole>(entity =>
        {
            entity.HasKey(e => e.EmployeeRoleId).HasName("PK__Employee__3461868636646C5A");

            entity.ToTable("EmployeeRole");

            entity.Property(e => e.EmployeeRoleId).HasColumnName("EmployeeRoleID");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Floor>(entity =>
        {
            entity.HasKey(e => e.FloorCode).HasName("PK__Floor__394E956B81A832F4");

            entity.ToTable("Floor");

            entity.HasIndex(e => e.ShelfCode, "IX_Floor_ShelfCode");

            entity.Property(e => e.FloorCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CurrentContainerCount).HasDefaultValue(0);
            entity.Property(e => e.CurrentWeight)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Height).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Length).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MaxContainers).HasDefaultValue(20);
            entity.Property(e => e.MaxWeight)
                .HasDefaultValue(500m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PositionX)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PositionY)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PositionZ)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ShelfCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.UtilizationRate)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Width).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.ShelfCodeNavigation).WithMany(p => p.Floors)
                .HasForeignKey(d => d.ShelfCode)
                .HasConstraintName("FK__Floor__ShelfCode__5FB337D6");
        });

        modelBuilder.Entity<FloorBlock>(entity =>
        {
            entity.HasKey(e => e.FloorBlockCode).HasName("PK__FloorBlo__DF861D583ABE3DA3");

            entity.ToTable("FloorBlock");

            entity.HasIndex(e => e.FloorCode, "IX_FloorBlock_FloorCode");

            entity.Property(e => e.FloorBlockCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FloorCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Height).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Length).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PositionX)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PositionY)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PositionZ)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Width).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.FloorCodeNavigation).WithMany(p => p.FloorBlocks)
                .HasForeignKey(d => d.FloorCode)
                .HasConstraintName("FK__FloorBloc__Floor__160F4887");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Item__727E83EB9C3E14D6");

            entity.ToTable("Item");

            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.AccessCount).HasDefaultValue(0);
            entity.Property(e => e.Brand).HasMaxLength(100);
            entity.Property(e => e.ContainerCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.EstimatedValue).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.FrequencyUse)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Low");
            entity.Property(e => e.Height).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("ImageURL");
            entity.Property(e => e.LastAccessDate).HasColumnType("datetime");
            entity.Property(e => e.Length).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.PlacementScore).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.PositionX).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PositionY).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PositionZ).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProductTypeId).HasColumnName("ProductTypeID");
            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.RotationAngle).HasDefaultValue(0);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.Volume)
                .HasComputedColumnSql("(([Length]*[Width])*[Height])", true)
                .HasColumnType("decimal(32, 6)");
            entity.Property(e => e.Weight).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Width).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.ContainerCodeNavigation).WithMany(p => p.Items)
                .HasForeignKey(d => d.ContainerCode)
                .HasConstraintName("FK__Item__ContainerC__3A4CA8FD");

            entity.HasOne(d => d.OrderDetail).WithMany(p => p.Items)
                .HasForeignKey(d => d.OrderDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Item__OrderDetai__395884C4");

            entity.HasOne(d => d.ProductType).WithMany(p => p.Items)
                .HasForeignKey(d => d.ProductTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Item__ProductTyp__3B40CD36");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderCode).HasName("PK__Order__999B52287CF5E3A0");

            entity.ToTable("Order");

            entity.HasIndex(e => e.CustomerCode, "IX_Order_CustomerCode");

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
                .HasPrincipalKey(p => p.CustomerCode)
                .HasForeignKey(d => d.CustomerCode)
                .HasConstraintName("FK_Order_CustomerCode");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D30C9215E8D7");

            entity.ToTable("OrderDetail");

            entity.HasIndex(e => e.ContainerCode, "IX_OrderDetail_ContainerCode");

            entity.HasIndex(e => e.OrderCode, "IX_OrderDetail_OrderCode");

            entity.HasIndex(e => e.ServiceId, "IX_OrderDetail_ServiceID");

            entity.HasIndex(e => e.StorageCode, "IX_OrderDetail_StorageCode");

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

        modelBuilder.Entity<PaymentHistory>(entity =>
        {
            entity.HasKey(e => e.PaymentHistoryCode).HasName("PK__PaymentH__F83BEDA4194B7DFF");

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
                .HasConstraintName("FK__PaymentHi__Order__29221CFB");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.ProductTypeId).HasName("PK__ProductT__A1312F4E69BB1C0D");

            entity.ToTable("ProductType");

            entity.Property(e => e.ProductTypeId).HasColumnName("ProductTypeID");
            entity.Property(e => e.AverageWeight).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.AvoidSunlight).HasDefaultValue(false);
            entity.Property(e => e.CanStack).HasDefaultValue(true);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.IsFragile).HasDefaultValue(false);
            entity.Property(e => e.MaxStackLayers).HasDefaultValue(5);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PlacementPriority).HasDefaultValue(5);
            entity.Property(e => e.PreferredZone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RequireMoistureControl).HasDefaultValue(false);
            entity.Property(e => e.RequireVentilation).HasDefaultValue(false);
            entity.Property(e => e.ShapeType)
                .HasMaxLength(20)
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

            entity.HasIndex(e => e.StorageCode, "IX_Shelf_StorageCode");

            entity.Property(e => e.ShelfCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Height).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Length).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PositionX)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PositionY)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PositionZ)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StorageCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Width).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.StorageCodeNavigation).WithMany(p => p.Shelves)
                .HasForeignKey(d => d.StorageCode)
                .HasConstraintName("FK__Shelf__StorageCo__5CD6CB2B");
        });

        modelBuilder.Entity<Storage>(entity =>
        {
            entity.HasKey(e => e.StorageCode).HasName("PK__Storage__AD8F8BC7D5ABE087");

            entity.ToTable("Storage");

            entity.HasIndex(e => e.BuildingId, "IX_Storage_BuildingId");

            entity.HasIndex(e => e.ProductTypeId, "IX_Storage_ProductTypeID");

            entity.HasIndex(e => e.StorageTypeId, "IX_Storage_StorageTypeID");

            entity.Property(e => e.StorageCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BuildingCode).HasMaxLength(50);
            entity.Property(e => e.Height).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.LastOptimizedDate).HasColumnType("datetime");
            entity.Property(e => e.Length).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.OccupiedContainers).HasDefaultValue(0);
            entity.Property(e => e.ProductTypeId).HasColumnName("ProductTypeID");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StorageTypeId).HasColumnName("StorageTypeID");
            entity.Property(e => e.TotalContainers).HasDefaultValue(0);
            entity.Property(e => e.TotalVolume)
                .HasComputedColumnSql("(([Length]*[Width])*[Height])", false)
                .HasColumnType("decimal(32, 6)");
            entity.Property(e => e.UsedVolume)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(15, 2)");
            entity.Property(e => e.UtilizationRate)
                .HasComputedColumnSql("(case when ([Length]*[Width])*[Height]>(0) then ([UsedVolume]/(([Length]*[Width])*[Height]))*(100) else (0) end)", false)
                .HasColumnType("decimal(38, 15)");
            entity.Property(e => e.Width).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Building).WithMany(p => p.Storages)
                .HasForeignKey(d => d.BuildingId)
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

            entity.HasIndex(e => e.StorageCode, "IX_StorageBlock_StorageCode");

            entity.Property(e => e.StorageBlockCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Height).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Length).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StorageCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Width).HasColumnType("decimal(10, 2)");

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

            entity.HasIndex(e => e.OrderCode, "IX_TrackingHistory_OrderCode");

            entity.Property(e => e.TrackingHistoryId).HasColumnName("TrackingHistoryID");
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

            entity.HasIndex(e => e.WorkflowTemplateId, "IX_WorkflowStep_WorkflowTemplateID");

            entity.Property(e => e.WorkflowStepId).HasColumnName("WorkflowStepID");
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

            entity.HasIndex(e => e.StorageTypeId, "IX_WorkflowTemplate_StorageTypeID");

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
