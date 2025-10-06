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
                    BuildingCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    BuildingName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Area = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    FloorQuantity = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Building__D4DA0325478792F2", x => x.BuildingCode);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    CustomerCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Phone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    CustomerName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__0667852064317DAB", x => x.CustomerCode);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeRole",
                columns: table => new
                {
                    EmployeeRoleID = table.Column<int>(type: "int", nullable: false),
                    EmployeeRole = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employee__3461868632E1BC62", x => x.EmployeeRoleID);
                });

            migrationBuilder.CreateTable(
                name: "RoomType",
                columns: table => new
                {
                    RoomTypeID = table.Column<int>(type: "int", nullable: false),
                    RoomTypeName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RoomType__BCC896110CC5D09F", x => x.RoomTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Service",
                columns: table => new
                {
                    ServiceID = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Service__C51BB0EAAB265F58", x => x.ServiceID);
                });

            migrationBuilder.CreateTable(
                name: "Floor",
                columns: table => new
                {
                    FloorCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    BuildingCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    FloorNumber = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Floor__394E956B4D721C8D", x => x.FloorCode);
                    table.ForeignKey(
                        name: "FK__Floor__BuildingC__534D60F1",
                        column: x => x.BuildingCode,
                        principalTable: "Building",
                        principalColumn: "BuildingCode");
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
                    table.PrimaryKey("PK__Order__999B52280722FFBF", x => x.OrderCode);
                    table.ForeignKey(
                        name: "FK__Order__CustomerC__5EBF139D",
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
                    EmployeeName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    BuildingCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Phone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Username = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Password = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employee__1F6425494C433023", x => x.EmployeeCode);
                    table.ForeignKey(
                        name: "FK__Employee__Buildi__71D1E811",
                        column: x => x.BuildingCode,
                        principalTable: "Building",
                        principalColumn: "BuildingCode");
                    table.ForeignKey(
                        name: "FK__Employee__Employ__70DDC3D8",
                        column: x => x.EmployeeRoleID,
                        principalTable: "EmployeeRole",
                        principalColumn: "EmployeeRoleID");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowTemplate",
                columns: table => new
                {
                    WorkflowTemplateID = table.Column<int>(type: "int", nullable: false),
                    TemplateName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    RoomTypeID = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workflow__96E60A37CE12083A", x => x.WorkflowTemplateID);
                    table.ForeignKey(
                        name: "FK__WorkflowT__RoomT__4BAC3F29",
                        column: x => x.RoomTypeID,
                        principalTable: "RoomType",
                        principalColumn: "RoomTypeID");
                });

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    RoomCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    FloorCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    RoomTypeID = table.Column<int>(type: "int", nullable: true),
                    Length = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Width = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Room__4F9D5230A600B6D9", x => x.RoomCode);
                    table.ForeignKey(
                        name: "FK__Room__FloorCode__5629CD9C",
                        column: x => x.FloorCode,
                        principalTable: "Floor",
                        principalColumn: "FloorCode");
                    table.ForeignKey(
                        name: "FK__Room__RoomTypeID__571DF1D5",
                        column: x => x.RoomTypeID,
                        principalTable: "RoomType",
                        principalColumn: "RoomTypeID");
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
                    table.PrimaryKey("PK__PaymentH__F83BEDA4BA9A5F87", x => x.PaymentHistoryCode);
                    table.ForeignKey(
                        name: "FK__PaymentHi__Order__74AE54BC",
                        column: x => x.OrderCode,
                        principalTable: "Order",
                        principalColumn: "OrderCode");
                });

            migrationBuilder.CreateTable(
                name: "TrackingHistory",
                columns: table => new
                {
                    TrackingHistoryID = table.Column<int>(type: "int", nullable: false),
                    OrderCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    OldStatus = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    NewStatus = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    ActionType = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    CreateAt = table.Column<DateOnly>(type: "date", nullable: true),
                    CurrentAssign = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    NextAssign = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Image = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Tracking__26DB07A7040C89E7", x => x.TrackingHistoryID);
                    table.ForeignKey(
                        name: "FK__TrackingH__Order__6C190EBB",
                        column: x => x.OrderCode,
                        principalTable: "Order",
                        principalColumn: "OrderCode");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowStep",
                columns: table => new
                {
                    WorkflowStepID = table.Column<int>(type: "int", nullable: false),
                    WorkflowTemplateID = table.Column<int>(type: "int", nullable: true),
                    StepNumber = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workflow__3612140149770D3B", x => x.WorkflowStepID);
                    table.ForeignKey(
                        name: "FK__WorkflowS__Workf__4E88ABD4",
                        column: x => x.WorkflowTemplateID,
                        principalTable: "WorkflowTemplate",
                        principalColumn: "WorkflowTemplateID");
                });

            migrationBuilder.CreateTable(
                name: "Cell",
                columns: table => new
                {
                    CellCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    RoomCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Length = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Width = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(18,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cell__F6994BB72E832EBC", x => x.CellCode);
                    table.ForeignKey(
                        name: "FK__Cell__RoomCode__59FA5E80",
                        column: x => x.RoomCode,
                        principalTable: "Room",
                        principalColumn: "RoomCode");
                });

            migrationBuilder.CreateTable(
                name: "Box",
                columns: table => new
                {
                    BoxCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Length = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Width = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true),
                    CellCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Box__CB1C2B234E5BF880", x => x.BoxCode);
                    table.ForeignKey(
                        name: "FK__Box__CellCode__619B8048",
                        column: x => x.CellCode,
                        principalTable: "Cell",
                        principalColumn: "CellCode");
                });

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    OrderDetailID = table.Column<int>(type: "int", nullable: false),
                    OrderCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    RoomCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    BoxCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    ServiceID = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SubTotal = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Image = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OrderDet__D3B9D30CF88C1100", x => x.OrderDetailID);
                    table.ForeignKey(
                        name: "FK__OrderDeta__BoxCo__68487DD7",
                        column: x => x.BoxCode,
                        principalTable: "Box",
                        principalColumn: "BoxCode");
                    table.ForeignKey(
                        name: "FK__OrderDeta__Order__66603565",
                        column: x => x.OrderCode,
                        principalTable: "Order",
                        principalColumn: "OrderCode");
                    table.ForeignKey(
                        name: "FK__OrderDeta__RoomC__6754599E",
                        column: x => x.RoomCode,
                        principalTable: "Room",
                        principalColumn: "RoomCode");
                    table.ForeignKey(
                        name: "FK__OrderDeta__Servi__693CA210",
                        column: x => x.ServiceID,
                        principalTable: "Service",
                        principalColumn: "ServiceID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Box_CellCode",
                table: "Box",
                column: "CellCode");

            migrationBuilder.CreateIndex(
                name: "IX_Cell_RoomCode",
                table: "Cell",
                column: "RoomCode");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_BuildingCode",
                table: "Employee",
                column: "BuildingCode");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_EmployeeRoleID",
                table: "Employee",
                column: "EmployeeRoleID");

            migrationBuilder.CreateIndex(
                name: "IX_Floor_BuildingCode",
                table: "Floor",
                column: "BuildingCode");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CustomerCode",
                table: "Order",
                column: "CustomerCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_BoxCode",
                table: "OrderDetail",
                column: "BoxCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_OrderCode",
                table: "OrderDetail",
                column: "OrderCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_RoomCode",
                table: "OrderDetail",
                column: "RoomCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_ServiceID",
                table: "OrderDetail",
                column: "ServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentHistory_OrderCode",
                table: "PaymentHistory",
                column: "OrderCode");

            migrationBuilder.CreateIndex(
                name: "IX_Room_FloorCode",
                table: "Room",
                column: "FloorCode");

            migrationBuilder.CreateIndex(
                name: "IX_Room_RoomTypeID",
                table: "Room",
                column: "RoomTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_TrackingHistory_OrderCode",
                table: "TrackingHistory",
                column: "OrderCode");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStep_WorkflowTemplateID",
                table: "WorkflowStep",
                column: "WorkflowTemplateID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTemplate_RoomTypeID",
                table: "WorkflowTemplate",
                column: "RoomTypeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "PaymentHistory");

            migrationBuilder.DropTable(
                name: "TrackingHistory");

            migrationBuilder.DropTable(
                name: "WorkflowStep");

            migrationBuilder.DropTable(
                name: "EmployeeRole");

            migrationBuilder.DropTable(
                name: "Box");

            migrationBuilder.DropTable(
                name: "Service");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "WorkflowTemplate");

            migrationBuilder.DropTable(
                name: "Cell");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "Floor");

            migrationBuilder.DropTable(
                name: "RoomType");

            migrationBuilder.DropTable(
                name: "Building");
        }
    }
}
