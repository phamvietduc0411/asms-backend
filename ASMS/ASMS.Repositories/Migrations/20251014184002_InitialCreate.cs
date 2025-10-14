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
                    FloorQuantity = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Building", x => x.BuildingId);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    CustomerCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Phone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Password = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__066785204D979A27", x => x.CustomerCode);
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
                    ProductTypeID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true)
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
                name: "StorageType",
                columns: table => new
                {
                    StorageTypeID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StorageT__C94B8F7DD4F87BA1", x => x.StorageTypeID);
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
                        name: "FK__Order__CustomerC__6EF57B66",
                        column: x => x.CustomerCode,
                        principalTable: "Customer",
                        principalColumn: "CustomerCode");
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    EmployeeCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    EmployeeRoleID = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    BuildingId = table.Column<int>(type: "int", unicode: false, maxLength: 50, nullable: true),
                    Phone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Username = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Password = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employee__1F642549B4A50CB4", x => x.EmployeeCode);
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
                name: "ContainerType",
                columns: table => new
                {
                    ContainerTypeID = table.Column<int>(type: "int", nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    ProductTypeID = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Containe__46FA6FF98608AE3E", x => x.ContainerTypeID);
                    table.ForeignKey(
                        name: "FK__Container__Produ__628FA481",
                        column: x => x.ProductTypeID,
                        principalTable: "ProductType",
                        principalColumn: "ProductTypeID");
                });

            migrationBuilder.CreateTable(
                name: "Storage",
                columns: table => new
                {
                    StorageCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    BuildingId = table.Column<int>(type: "int", unicode: false, maxLength: 50, nullable: true),
                    StorageTypeID = table.Column<int>(type: "int", nullable: true),
                    ProductTypeID = table.Column<int>(type: "int", nullable: true),
                    Length = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Width = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true)
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
                name: "WorkflowTemplate",
                columns: table => new
                {
                    WorkflowTemplateID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    StorageTypeID = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workflow__96E60A37BE8B1C29", x => x.WorkflowTemplateID);
                    table.ForeignKey(
                        name: "FK__WorkflowT__Stora__4BAC3F29",
                        column: x => x.StorageTypeID,
                        principalTable: "StorageType",
                        principalColumn: "StorageTypeID");
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
                    Length = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Width = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(18,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Shelf__24D15D7509E5371F", x => x.ShelfCode);
                    table.ForeignKey(
                        name: "FK__Shelf__StorageCo__5CD6CB2B",
                        column: x => x.StorageCode,
                        principalTable: "Storage",
                        principalColumn: "StorageCode");
                });

            migrationBuilder.CreateTable(
                name: "StorageBlock",
                columns: table => new
                {
                    StorageBlockCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    StorageCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Length = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Width = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StorageB__10B0C0AD25AD5F9E", x => x.StorageBlockCode);
                    table.ForeignKey(
                        name: "FK__StorageBl__Stora__59FA5E80",
                        column: x => x.StorageCode,
                        principalTable: "Storage",
                        principalColumn: "StorageCode");
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
                name: "Floor",
                columns: table => new
                {
                    FloorCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ShelfCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    FloorNumber = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true),
                    Length = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Width = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(18,0)", nullable: true)
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
                    Length = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Width = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Containe__874FE471B6792E9D", x => x.ContainerCode);
                    table.ForeignKey(
                        name: "FK__Container__Floor__656C112C",
                        column: x => x.FloorCode,
                        principalTable: "Floor",
                        principalColumn: "FloorCode");
                });

            migrationBuilder.CreateTable(
                name: "FloorBlock",
                columns: table => new
                {
                    FloorBlockCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    FloorCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Length = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Width = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__FloorBlo__DF861D583ABE3DA3", x => x.FloorBlockCode);
                    table.ForeignKey(
                        name: "FK__FloorBloc__Floor__160F4887",
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
                    Assign = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    UpdatedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    OldFloor = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    CurrentFloor = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_Container_FloorCode",
                table: "Container",
                column: "FloorCode");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerLocationLog_ContainerCode",
                table: "ContainerLocationLog",
                column: "ContainerCode");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerType_ProductTypeID",
                table: "ContainerType",
                column: "ProductTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_BuildingId",
                table: "Employee",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_EmployeeRoleID",
                table: "Employee",
                column: "EmployeeRoleID");

            migrationBuilder.CreateIndex(
                name: "IX_Floor_ShelfCode",
                table: "Floor",
                column: "ShelfCode");

            migrationBuilder.CreateIndex(
                name: "IX_FloorBlock_FloorCode",
                table: "FloorBlock",
                column: "FloorCode");

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
                name: "IX_StorageBlock_StorageCode",
                table: "StorageBlock",
                column: "StorageCode");

            migrationBuilder.CreateIndex(
                name: "IX_TrackingHistory_OrderCode",
                table: "TrackingHistory",
                column: "OrderCode");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStep_WorkflowTemplateID",
                table: "WorkflowStep",
                column: "WorkflowTemplateID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTemplate_StorageTypeID",
                table: "WorkflowTemplate",
                column: "StorageTypeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContainerLocationLog");

            migrationBuilder.DropTable(
                name: "ContainerType");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "FloorBlock");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "StorageBlock");

            migrationBuilder.DropTable(
                name: "TrackingHistory");

            migrationBuilder.DropTable(
                name: "WorkflowStep");

            migrationBuilder.DropTable(
                name: "EmployeeRole");

            migrationBuilder.DropTable(
                name: "Container");

            migrationBuilder.DropTable(
                name: "Service");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "WorkflowTemplate");

            migrationBuilder.DropTable(
                name: "Floor");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Shelf");

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
