using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASMS.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Building",
                columns: table => new
                {
                    BuildingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuildingCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Area = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Building", x => x.BuildingId);
                });

            migrationBuilder.CreateTable(
                name: "ContainerType",
                columns: table => new
                {
                    ContainerTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    Length = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Width = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Containe__46FA6FD9D2C4FA11", x => x.ContainerTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Phone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: ""),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: ""),
                    Password = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                    table.UniqueConstraint("AK_Customer_CustomerCode", x => x.CustomerCode);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeRole",
                columns: table => new
                {
                    EmployeeRoleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employee__3461868636646C5A", x => x.EmployeeRoleID);
                });

            migrationBuilder.CreateTable(
                name: "ProductType",
                columns: table => new
                {
                    ProductTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true),
                    IsFragile = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CanStack = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProductT__A1312F4E69BB1C0D", x => x.ProductTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Service",
                columns: table => new
                {
                    ServiceID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Service__C51BB0EA2D1E364C", x => x.ServiceID);
                });

            migrationBuilder.CreateTable(
                name: "ShelfType",
                columns: table => new
                {
                    ShelfTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Length = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Width = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Height = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ShelfTyp__50AF6654E81B2766", x => x.ShelfTypeId);
                });

            migrationBuilder.CreateTable(
                name: "StorageType",
                columns: table => new
                {
                    StorageTypeID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Length = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Width = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TotalVolume = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Area = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StorageT__C94B8F7DD4F87BA1", x => x.StorageTypeID);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowTemplate",
                columns: table => new
                {
                    WorkflowTemplateID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workflow__96E60A37BE8B1C29", x => x.WorkflowTemplateID);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    OrderCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    CustomerCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    OrderDate = table.Column<DateOnly>(type: "date", nullable: true),
                    DepositDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ReturnDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    PaymentStatus = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    UnpaidAmount = table.Column<decimal>(type: "decimal(18,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Order__999B52287CF5E3A0", x => x.OrderCode);
                    table.ForeignKey(
                        name: "FK_Order_CustomerCode",
                        column: x => x.CustomerCode,
                        principalTable: "Customer",
                        principalColumn: "CustomerCode");
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    EmployeeRoleID = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    BuildingId = table.Column<int>(type: "int", nullable: true),
                    Phone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Username = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Password = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employee__1F642549B4A50CB4", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Employee__Buildi__7D439ABD",
                        column: x => x.BuildingId,
                        principalTable: "Building",
                        principalColumn: "BuildingId");
                    table.ForeignKey(
                        name: "FK__Employee__Employ__7C4F7684",
                        column: x => x.EmployeeRoleID,
                        principalTable: "EmployeeRole",
                        principalColumn: "EmployeeRoleID");
                });

            migrationBuilder.CreateTable(
                name: "Storage",
                columns: table => new
                {
                    StorageCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    BuildingId = table.Column<int>(type: "int", nullable: true),
                    StorageTypeID = table.Column<int>(type: "int", nullable: true),
                    ProductTypeID = table.Column<int>(type: "int", nullable: true),
                    Length = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Width = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true),
                    UsedVolume = table.Column<decimal>(type: "decimal(15,2)", nullable: true, defaultValue: 0m),
                    TotalContainers = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    OccupiedContainers = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    LastOptimizedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    BuildingCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TotalVolume = table.Column<decimal>(type: "decimal(32,6)", nullable: true, computedColumnSql: "(([Length]*[Width])*[Height])", stored: false),
                    UtilizationRate = table.Column<decimal>(type: "decimal(38,15)", nullable: true, computedColumnSql: "(case when ([Length]*[Width])*[Height]>(0) then ([UsedVolume]/(([Length]*[Width])*[Height]))*(100) else (0) end)", stored: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Storage__AD8F8BC7D5ABE087", x => x.StorageCode);
                    table.ForeignKey(
                        name: "FK__Storage__Buildin__5535A963",
                        column: x => x.BuildingId,
                        principalTable: "Building",
                        principalColumn: "BuildingId");
                    table.ForeignKey(
                        name: "FK__Storage__Product__571DF1D5",
                        column: x => x.ProductTypeID,
                        principalTable: "ProductType",
                        principalColumn: "ProductTypeID");
                    table.ForeignKey(
                        name: "FK__Storage__Storage__5629CD9C",
                        column: x => x.StorageTypeID,
                        principalTable: "StorageType",
                        principalColumn: "StorageTypeID");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowStep",
                columns: table => new
                {
                    WorkflowStepID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowTemplateID = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    StepNumber = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workflow__36121401BCF42ECF", x => x.WorkflowStepID);
                    table.ForeignKey(
                        name: "FK__WorkflowS__Workf__4E88ABD4",
                        column: x => x.WorkflowTemplateID,
                        principalTable: "WorkflowTemplate",
                        principalColumn: "WorkflowTemplateID");
                });

            migrationBuilder.CreateTable(
                name: "PaymentHistory",
                columns: table => new
                {
                    PaymentHistoryCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    OrderCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    PaymentMethod = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    PaymentPlatform = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PaymentH__F83BEDA4194B7DFF", x => x.PaymentHistoryCode);
                    table.ForeignKey(
                        name: "FK__PaymentHi__Order__29221CFB",
                        column: x => x.OrderCode,
                        principalTable: "Order",
                        principalColumn: "OrderCode");
                });

            migrationBuilder.CreateTable(
                name: "TrackingHistory",
                columns: table => new
                {
                    TrackingHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDetailCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    OldStatus = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    NewStatus = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    ActionType = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    CreateAt = table.Column<DateOnly>(type: "date", nullable: true),
                    CurrentAssign = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    NextAssign = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Image = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    OrderCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Tracking__26DB07A738199CF4", x => x.TrackingHistoryID);
                    table.ForeignKey(
                        name: "FK__TrackingH__Order__778AC167",
                        column: x => x.OrderCode,
                        principalTable: "Order",
                        principalColumn: "OrderCode");
                });

            migrationBuilder.CreateTable(
                name: "Shelf",
                columns: table => new
                {
                    ShelfCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    StorageCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true),
                    Length = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Width = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ShelfTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Shelf__24D15D7509E5371F", x => x.ShelfCode);
                    table.ForeignKey(
                        name: "FK_Shelf_ShelfType",
                        column: x => x.ShelfTypeId,
                        principalTable: "ShelfType",
                        principalColumn: "ShelfTypeId");
                    table.ForeignKey(
                        name: "FK__Shelf__StorageCo__5CD6CB2B",
                        column: x => x.StorageCode,
                        principalTable: "Storage",
                        principalColumn: "StorageCode");
                });

            migrationBuilder.CreateTable(
                name: "Floor",
                columns: table => new
                {
                    FloorCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ShelfCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    FloorNumber = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true),
                    Length = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Width = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    MaxWeight = table.Column<decimal>(type: "decimal(10,2)", nullable: true, defaultValue: 500m),
                    CurrentWeight = table.Column<decimal>(type: "decimal(10,2)", nullable: true, defaultValue: 0m),
                    MaxContainers = table.Column<int>(type: "int", nullable: true, defaultValue: 20),
                    CurrentContainerCount = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    UtilizationRate = table.Column<decimal>(type: "decimal(5,2)", nullable: true, defaultValue: 0m),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Floor__394E956B81A832F4", x => x.FloorCode);
                    table.ForeignKey(
                        name: "FK__Floor__ShelfCode__5FB337D6",
                        column: x => x.ShelfCode,
                        principalTable: "Shelf",
                        principalColumn: "ShelfCode");
                });

            migrationBuilder.CreateTable(
                name: "Container",
                columns: table => new
                {
                    ContainerCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    FloorCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    ProductTypeID = table.Column<int>(type: "int", nullable: true),
                    MaxWeight = table.Column<decimal>(type: "decimal(10,2)", nullable: true, defaultValue: 100m),
                    CurrentWeight = table.Column<decimal>(type: "decimal(10,2)", nullable: true, defaultValue: 0m),
                    PositionX = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    PositionY = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    PositionZ = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    LastOptimizedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    OptimizationScore = table.Column<decimal>(type: "decimal(5,2)", nullable: true, defaultValue: 0m),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ContainerTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Containe__874FE471B6792E9D", x => x.ContainerCode);
                    table.ForeignKey(
                        name: "FK_Container_ContainerType",
                        column: x => x.ContainerTypeId,
                        principalTable: "ContainerType",
                        principalColumn: "ContainerTypeId");
                    table.ForeignKey(
                        name: "FK_Container_ProductType",
                        column: x => x.ProductTypeID,
                        principalTable: "ProductType",
                        principalColumn: "ProductTypeID");
                    table.ForeignKey(
                        name: "FK__Container__Floor__656C112C",
                        column: x => x.FloorCode,
                        principalTable: "Floor",
                        principalColumn: "FloorCode");
                });

            migrationBuilder.CreateTable(
                name: "ContainerLocationLog",
                columns: table => new
                {
                    ContainerLocationLogID = table.Column<int>(type: "int", nullable: false),
                    ContainerCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    OrderCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    PerformedBy = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    UpdatedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    OldFloor = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    CurrentFloor = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Algorithm = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Containe__06E7FAAA2A79C5C0", x => x.ContainerLocationLogID);
                    table.ForeignKey(
                        name: "FK__Container__Conta__68487DD7",
                        column: x => x.ContainerCode,
                        principalTable: "Container",
                        principalColumn: "ContainerCode");
                });

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    OrderDetailID = table.Column<int>(type: "int", nullable: false),
                    OrderCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    StorageCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    ContainerCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    ServiceID = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Quantity = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SubTotal = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Image = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OrderDet__D3B9D30C9215E8D7", x => x.OrderDetailID);
                    table.ForeignKey(
                        name: "FK__OrderDeta__Conta__73BA3083",
                        column: x => x.ContainerCode,
                        principalTable: "Container",
                        principalColumn: "ContainerCode");
                    table.ForeignKey(
                        name: "FK__OrderDeta__Order__71D1E811",
                        column: x => x.OrderCode,
                        principalTable: "Order",
                        principalColumn: "OrderCode");
                    table.ForeignKey(
                        name: "FK__OrderDeta__Servi__74AE54BC",
                        column: x => x.ServiceID,
                        principalTable: "Service",
                        principalColumn: "ServiceID");
                    table.ForeignKey(
                        name: "FK__OrderDeta__Stora__72C60C4A",
                        column: x => x.StorageCode,
                        principalTable: "Storage",
                        principalColumn: "StorageCode");
                });

            migrationBuilder.CreateTable(
                name: "OrderDetailProductType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDetailId = table.Column<int>(type: "int", nullable: false),
                    ProductTypeId = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OrderDet__3214EC07CB864502", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetailProductType_OrderDetail",
                        column: x => x.OrderDetailId,
                        principalTable: "OrderDetail",
                        principalColumn: "OrderDetailID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetailProductType_ProductType",
                        column: x => x.ProductTypeId,
                        principalTable: "ProductType",
                        principalColumn: "ProductTypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Container_ContainerTypeId",
                table: "Container",
                column: "ContainerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Container_FloorCode",
                table: "Container",
                column: "FloorCode");

            migrationBuilder.CreateIndex(
                name: "IX_Container_ProductTypeID",
                table: "Container",
                column: "ProductTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerLocationLog_Container_Date",
                table: "ContainerLocationLog",
                columns: new[] { "ContainerCode", "UpdatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ContainerLocationLog_ContainerCode",
                table: "ContainerLocationLog",
                column: "ContainerCode");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerLocationLog_Order",
                table: "ContainerLocationLog",
                column: "OrderCode");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_BuildingId",
                table: "Employees",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeRoleID",
                table: "Employees",
                column: "EmployeeRoleID");

            migrationBuilder.CreateIndex(
                name: "IX_Floor_ShelfCode",
                table: "Floor",
                column: "ShelfCode");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CustomerCode",
                table: "Order",
                column: "CustomerCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_ContainerCode",
                table: "OrderDetail",
                column: "ContainerCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_OrderCode",
                table: "OrderDetail",
                column: "OrderCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_ServiceID",
                table: "OrderDetail",
                column: "ServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_StorageCode",
                table: "OrderDetail",
                column: "StorageCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetailProductType_OrderDetailId",
                table: "OrderDetailProductType",
                column: "OrderDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetailProductType_ProductTypeId",
                table: "OrderDetailProductType",
                column: "ProductTypeId");

            migrationBuilder.CreateIndex(
                name: "UQ_OrderDetailProductType_OrderDetail_ProductType",
                table: "OrderDetailProductType",
                columns: new[] { "OrderDetailId", "ProductTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentHistory_OrderCode",
                table: "PaymentHistory",
                column: "OrderCode");

            migrationBuilder.CreateIndex(
                name: "IX_Shelf_ShelfTypeId",
                table: "Shelf",
                column: "ShelfTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Shelf_StorageCode",
                table: "Shelf",
                column: "StorageCode");

            migrationBuilder.CreateIndex(
                name: "IX_Storage_BuildingId",
                table: "Storage",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Storage_ProductTypeID",
                table: "Storage",
                column: "ProductTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Storage_StorageTypeID",
                table: "Storage",
                column: "StorageTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_TrackingHistory_OrderCode",
                table: "TrackingHistory",
                column: "OrderCode");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStep_WorkflowTemplateID",
                table: "WorkflowStep",
                column: "WorkflowTemplateID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContainerLocationLog");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "OrderDetailProductType");

            migrationBuilder.DropTable(
                name: "PaymentHistory");

            migrationBuilder.DropTable(
                name: "TrackingHistory");

            migrationBuilder.DropTable(
                name: "WorkflowStep");

            migrationBuilder.DropTable(
                name: "EmployeeRole");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "WorkflowTemplate");

            migrationBuilder.DropTable(
                name: "Container");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Service");

            migrationBuilder.DropTable(
                name: "ContainerType");

            migrationBuilder.DropTable(
                name: "Floor");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Shelf");

            migrationBuilder.DropTable(
                name: "ShelfType");

            migrationBuilder.DropTable(
                name: "Storage");

            migrationBuilder.DropTable(
                name: "Building");

            migrationBuilder.DropTable(
                name: "ProductType");

            migrationBuilder.DropTable(
                name: "StorageType");
        }
    }
}
