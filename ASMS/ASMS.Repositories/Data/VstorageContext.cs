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

    public virtual DbSet<ContainerType> ContainerTypes { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeRole> EmployeeRoles { get; set; }

    public virtual DbSet<Floor> Floors { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderDetailProductType> OrderDetailProductTypes { get; set; }

    public virtual DbSet<OrderDetailService> OrderDetailServices { get; set; }

    public virtual DbSet<PaymentHistory> PaymentHistories { get; set; }


    public virtual DbSet<ProductType> ProductTypes { get; set; }
    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Shelf> Shelves { get; set; }

    public virtual DbSet<ShelfType> ShelfTypes { get; set; }

    public virtual DbSet<Storage> Storages { get; set; }

    public virtual DbSet<StorageType> StorageTypes { get; set; }

    public virtual DbSet<TrackingHistory> TrackingHistories { get; set; }

    public virtual DbSet<WorkflowStep> WorkflowSteps { get; set; }

    public virtual DbSet<WorkflowTemplate> WorkflowTemplates { get; set; }

    public virtual DbSet<RefreshToken> RefreshToken { get; set; }

    public virtual DbSet<PaymentResult> PaymentResults { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //=> optionsBuilder.UseSqlServer("Server=ROG-ZEPHYRUS-G1\\VIETDUC;Database=VStorage;Uid=sa;Pwd=123456;Trusted_Connection=True;TrustServerCertificate=True");
    //=> optionsBuilder.UseSqlServer("Server=LAPTOP-39B7IASC\\SQLEXPRESS;Database=VStoragePublic;Uid=sa;Pwd=1;Trusted_Connection=True;TrustServerCertificate=True;");
    => optionsBuilder.UseSqlServer("Server=tcp:asmsdb.database.windows.net,1433;Initial Catalog=VStoragePublic;Persist Security Info=False;User ID=asmsadminlogin;Password=@Testpassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");

    // => optionsBuilder.UseSqlServer("Server=DESKTOP-F3F1PD5\\SQLEXPRESS;Database=VStorage;Uid=sa;Pwd=12345;Trusted_Connection=True;TrustServerCertificate=True");
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
            entity.Property(e => e.ImageUrl).HasMaxLength(1000);
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
            entity.Property(e => e.ContainerAboveCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CurrentWeight)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.FloorCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ImageUrl).HasMaxLength(1000);
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.LastOptimizedDate).HasColumnType("datetime");
            entity.Property(e => e.MaxWeight)
                .HasDefaultValue(100m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.OptimizationScore)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.PositionX).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PositionY).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PositionZ).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ProductTypeId).HasColumnName("ProductTypeID");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.ContainerType).WithMany(p => p.Containers)
                .HasForeignKey(d => d.ContainerTypeId)
                .HasConstraintName("FK_Container_ContainerType");

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
                .ValueGeneratedOnAdd()
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

        modelBuilder.Entity<ContainerType>(entity =>
        {
            entity.HasKey(e => e.ContainerTypeId).HasName("PK__Containe__46FA6FD9D2C4FA11");

            entity.ToTable("ContainerType");

            entity.Property(e => e.Height).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ImageUrl).HasMaxLength(1000);
            entity.Property(e => e.Length).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Type)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Width).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customer");



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
            entity.Property(e => e.OrderActionCount).HasDefaultValue(0);
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
            entity.Property(e => e.ImageUrl).HasMaxLength(1000);
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Length).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MaxContainers).HasDefaultValue(20);
            entity.Property(e => e.MaxWeight)
                .HasDefaultValue(500m)
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

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderCode).HasName("PK__Order__999B52287CF5E3A0");

            entity.ToTable("Order");

            entity.HasIndex(e => e.CustomerCode, "IX_Order_CustomerCode");

            entity.Property(e => e.OrderCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Address).HasMaxLength(1000);
            entity.Property(e => e.BuildingCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CustomerCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CustomerName).HasMaxLength(1000);
            entity.Property(e => e.Email).HasMaxLength(500);
            entity.Property(e => e.Image).HasMaxLength(1000);
            entity.Property(e => e.Note).HasMaxLength(1000);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PhoneContact).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Style).HasMaxLength(50);
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

            entity.HasIndex(e => e.StorageCode, "IX_OrderDetail_StorageCode");

            entity.Property(e => e.OrderDetailId)
                .ValueGeneratedOnAdd()
                .HasColumnName("OrderDetailID");
            entity.Property(e => e.ContainerCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Image)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.IsPlaced).HasColumnName("isPlaced");
            entity.Property(e => e.OrderCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Quantity).HasMaxLength(500);
            entity.Property(e => e.StorageCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StorageTypeId).HasColumnName("StorageTypeID");
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.ContainerCodeNavigation).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ContainerCode)
                .HasConstraintName("FK__OrderDeta__Conta__73BA3083");

            entity.HasOne(d => d.OrderCodeNavigation).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderCode)
                .HasConstraintName("FK__OrderDeta__Order__71D1E811");

            entity.HasOne(d => d.StorageCodeNavigation).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.StorageCode)
                .HasConstraintName("FK__OrderDeta__Stora__72C60C4A");
        });

        modelBuilder.Entity<OrderDetailProductType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderDet__3214EC07CB864502");

            entity.ToTable("OrderDetailProductType");

            entity.HasIndex(e => e.OrderDetailId, "IX_OrderDetailProductType_OrderDetailId");

            entity.HasIndex(e => e.ProductTypeId, "IX_OrderDetailProductType_ProductTypeId");

            entity.HasIndex(e => new { e.OrderDetailId, e.ProductTypeId }, "UQ_OrderDetailProductType_OrderDetail_ProductType").IsUnique();

            entity.Property(e => e.IsActive).HasColumnName("isActive");

            entity.HasOne(d => d.OrderDetail).WithMany(p => p.OrderDetailProductTypes)
                .HasForeignKey(d => d.OrderDetailId)
                .HasConstraintName("FK_OrderDetailProductType_OrderDetail");

            entity.HasOne(d => d.ProductType).WithMany(p => p.OrderDetailProductTypes)
                .HasForeignKey(d => d.ProductTypeId)
                .HasConstraintName("FK_OrderDetailProductType_ProductType");
        });

        modelBuilder.Entity<OrderDetailService>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderDet__3214EC07CD915A73");

            entity.ToTable("OrderDetailService");

            entity.HasIndex(e => new { e.OrderDetailId, e.ServiceId }, "UQ_OrderDetailService_OrderDetail_Service").IsUnique();

            entity.HasOne(d => d.OrderDetail).WithMany(p => p.OrderDetailServices)
                .HasForeignKey(d => d.OrderDetailId)
                .HasConstraintName("FK_OrderDetailService_OrderDetail");

            entity.HasOne(d => d.Service).WithMany(p => p.OrderDetailServices)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_OrderDetailService_Service");
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

        modelBuilder.Entity<PaymentResult>(entity =>
        {
            entity.ToTable("PaymentResults");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.PaymentCode).HasMaxLength(50).IsRequired();
            entity.Property(e => e.OrderCode).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Message).HasMaxLength(500);
            entity.Property(e => e.Url).HasMaxLength(1000);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("GETUTCDATE()");

            // FK
            entity.HasOne(d => d.Order)
                .WithMany(p => p.PaymentResults)
                .HasForeignKey(d => d.OrderCode)
                .HasConstraintName("FK_PaymentResults_Order");
        });


        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.ProductTypeId).HasName("PK__ProductT__A1312F4E69BB1C0D");

            entity.ToTable("ProductType");

            entity.Property(e => e.ProductTypeId).HasColumnName("ProductTypeID");
            entity.Property(e => e.CanStack).HasDefaultValue(true);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.IsFragile).HasDefaultValue(false);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Vname)
                .HasMaxLength(200)
                .HasColumnName("VName");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Service__C51BB0EA2D1E364C");

            entity.ToTable("Service");

            entity.Property(e => e.ServiceId)
                .ValueGeneratedNever()
                .HasColumnName("ServiceID");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Vname)
                .HasMaxLength(200)
                .HasColumnName("VName");
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
            entity.Property(e => e.ImageUrl).HasMaxLength(1000);
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Length).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StorageCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Width).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.ShelfType).WithMany(p => p.Shelves)
                .HasForeignKey(d => d.ShelfTypeId)
                .HasConstraintName("FK_Shelf_ShelfType");

            entity.HasOne(d => d.StorageCodeNavigation).WithMany(p => p.Shelves)
                .HasForeignKey(d => d.StorageCode)
                .HasConstraintName("FK__Shelf__StorageCo__5CD6CB2B");
        });

        modelBuilder.Entity<ShelfType>(entity =>
        {
            entity.HasKey(e => e.ShelfTypeId).HasName("PK__ShelfTyp__50AF6654E81B2766");

            entity.ToTable("ShelfType");

            entity.Property(e => e.Height).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImageUrl).HasMaxLength(1000);
            entity.Property(e => e.Length).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Width).HasColumnType("decimal(18, 2)");
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
            entity.Property(e => e.ImageUrl).HasMaxLength(1000);
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

        modelBuilder.Entity<StorageType>(entity =>
        {
            entity.HasKey(e => e.StorageTypeId).HasName("PK__StorageT__C94B8F7DD4F87BA1");

            entity.ToTable("StorageType");

            entity.Property(e => e.StorageTypeId)
                .ValueGeneratedNever()
                .HasColumnName("StorageTypeID");
            entity.Property(e => e.Area).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Height).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ImageUrl).HasMaxLength(1000);
            entity.Property(e => e.Length).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalVolume).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Width).HasColumnType("decimal(10, 2)");
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

            entity.Property(e => e.WorkflowTemplateId)
                .ValueGeneratedNever()
                .HasColumnName("WorkflowTemplateID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<RefreshToken>()
        .HasOne(rt => rt.Employee)
        .WithMany(e => e.RefreshTokens)
        .HasForeignKey(rt => rt.EmployeeId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.Customer)
            .WithMany(c => c.RefreshTokens)
            .HasForeignKey(rt => rt.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
