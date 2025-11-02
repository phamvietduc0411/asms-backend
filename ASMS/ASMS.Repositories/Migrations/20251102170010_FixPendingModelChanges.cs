using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASMS.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class FixPendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContainerType");

            migrationBuilder.RenameColumn(
                name: "Assign",
                table: "ContainerLocationLog",
                newName: "PerformedBy");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastOptimizedDate",
                table: "Storage",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OccupiedContainers",
                table: "Storage",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalContainers",
                table: "Storage",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "UsedVolume",
                table: "Storage",
                type: "decimal(15,2)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AverageWeight",
                table: "ProductType",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AvoidSunlight",
                table: "ProductType",
                type: "bit",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanStack",
                table: "ProductType",
                type: "bit",
                nullable: true,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ProductType",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFragile",
                table: "ProductType",
                type: "bit",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxStackLayers",
                table: "ProductType",
                type: "int",
                nullable: true,
                defaultValue: 5);

            migrationBuilder.AddColumn<int>(
                name: "PlacementPriority",
                table: "ProductType",
                type: "int",
                nullable: true,
                defaultValue: 5);

            migrationBuilder.AddColumn<string>(
                name: "PreferredZone",
                table: "ProductType",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequireMoistureControl",
                table: "ProductType",
                type: "bit",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequireVentilation",
                table: "ProductType",
                type: "bit",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ShapeType",
                table: "ProductType",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentContainerCount",
                table: "Floor",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentWeight",
                table: "Floor",
                type: "decimal(10,2)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "MaxContainers",
                table: "Floor",
                type: "int",
                nullable: true,
                defaultValue: 20);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxWeight",
                table: "Floor",
                type: "decimal(10,2)",
                nullable: true,
                defaultValue: 500m);

            migrationBuilder.AddColumn<decimal>(
                name: "PositionX",
                table: "Floor",
                type: "decimal(10,2)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PositionY",
                table: "Floor",
                type: "decimal(10,2)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PositionZ",
                table: "Floor",
                type: "decimal(10,2)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UtilizationRate",
                table: "Floor",
                type: "decimal(5,2)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Customer",
                type: "varchar(500)",
                unicode: false,
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldUnicode: false,
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Customer",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Customer",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "Algorithm",
                table: "ContainerLocationLog",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "ContainerLocationLog",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "ContainerLocationLog",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentItemCount",
                table: "Container",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentWeight",
                table: "Container",
                type: "decimal(10,2)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "HasFragileItems",
                table: "Container",
                type: "bit",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasHeavyItems",
                table: "Container",
                type: "bit",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastOptimizedDate",
                table: "Container",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxItems",
                table: "Container",
                type: "int",
                nullable: true,
                defaultValue: 50);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxWeight",
                table: "Container",
                type: "decimal(10,2)",
                nullable: true,
                defaultValue: 100m);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Container",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OptimizationScore",
                table: "Container",
                type: "decimal(5,2)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PositionX",
                table: "Container",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PositionY",
                table: "Container",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PositionZ",
                table: "Container",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Container",
                type: "decimal(18,0)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductTypeID",
                table: "Container",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RotationAngle",
                table: "Container",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "UsedVolume",
                table: "Container",
                type: "decimal(15,2)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalVolume",
                table: "Storage",
                type: "decimal(38,0)",
                nullable: true,
                computedColumnSql: "(([Length]*[Width])*[Height])",
                stored: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UtilizationRate",
                table: "Storage",
                type: "decimal(38,21)",
                nullable: true,
                computedColumnSql: "(case when ([Length]*[Width])*[Height]>(0) then ([UsedVolume]/(([Length]*[Width])*[Height]))*(100) else (0) end)",
                stored: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalVolume",
                table: "Container",
                type: "decimal(38,0)",
                nullable: true,
                computedColumnSql: "(([Length]*[Width])*[Height])",
                stored: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UtilizationRate",
                table: "Container",
                type: "decimal(38,21)",
                nullable: true,
                computedColumnSql: "(case when ([Length]*[Width])*[Height]>(0) then ([UsedVolume]/(([Length]*[Width])*[Height]))*(100) else (0) end)",
                stored: true);

            migrationBuilder.CreateTable(
                name: "Item",
                columns: table => new
                {
                    ItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDetailID = table.Column<int>(type: "int", nullable: false),
                    ContainerCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    ProductTypeID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Length = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Width = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Volume = table.Column<decimal>(type: "decimal(32,6)", nullable: true, computedColumnSql: "(([Length]*[Width])*[Height])", stored: true),
                    Quantity = table.Column<int>(type: "int", nullable: true, defaultValue: 1),
                    IsFragile = table.Column<bool>(type: "bit", nullable: true),
                    CanStack = table.Column<bool>(type: "bit", nullable: true),
                    FrequencyUse = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, defaultValue: "Low"),
                    LastAccessDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    AccessCount = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    PositionX = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    PositionY = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    PositionZ = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    RotationAngle = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    PlacementScore = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    ImageURL = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    EstimatedValue = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, defaultValue: "Pending"),
                    CreatedBy = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Item__727E83EB9C3E14D6", x => x.ItemID);
                    table.ForeignKey(
                        name: "FK__Item__ContainerC__3A4CA8FD",
                        column: x => x.ContainerCode,
                        principalTable: "Container",
                        principalColumn: "ContainerCode");
                    table.ForeignKey(
                        name: "FK__Item__OrderDetai__395884C4",
                        column: x => x.OrderDetailID,
                        principalTable: "OrderDetail",
                        principalColumn: "OrderDetailID");
                    table.ForeignKey(
                        name: "FK__Item__ProductTyp__3B40CD36",
                        column: x => x.ProductTypeID,
                        principalTable: "ProductType",
                        principalColumn: "ProductTypeID");
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

            //migrationBuilder.CreateIndex(
            //    name: "AK_Customer_CustomerCode",
            //    table: "Customer",
            //    column: "CustomerCode",
            //    unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContainerLocationLog_Container_Date",
                table: "ContainerLocationLog",
                columns: new[] { "ContainerCode", "UpdatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ContainerLocationLog_Order",
                table: "ContainerLocationLog",
                column: "OrderCode");

            migrationBuilder.CreateIndex(
                name: "IX_Container_ProductTypeID",
                table: "Container",
                column: "ProductTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Item_ContainerCode",
                table: "Item",
                column: "ContainerCode");

            migrationBuilder.CreateIndex(
                name: "IX_Item_OrderDetailID",
                table: "Item",
                column: "OrderDetailID");

            migrationBuilder.CreateIndex(
                name: "IX_Item_ProductTypeID",
                table: "Item",
                column: "ProductTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentHistory_OrderCode",
                table: "PaymentHistory",
                column: "OrderCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Container_ProductType",
                table: "Container",
                column: "ProductTypeID",
                principalTable: "ProductType",
                principalColumn: "ProductTypeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Container_ProductType",
                table: "Container");

            migrationBuilder.DropTable(
                name: "Item");

            migrationBuilder.DropTable(
                name: "PaymentHistory");

            //migrationBuilder.DropIndex(
            //    name: "AK_Customer_CustomerCode",
            //    table: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_ContainerLocationLog_Container_Date",
                table: "ContainerLocationLog");

            migrationBuilder.DropIndex(
                name: "IX_ContainerLocationLog_Order",
                table: "ContainerLocationLog");

            migrationBuilder.DropIndex(
                name: "IX_Container_ProductTypeID",
                table: "Container");

            migrationBuilder.DropColumn(
                name: "TotalVolume",
                table: "Storage");

            migrationBuilder.DropColumn(
                name: "UtilizationRate",
                table: "Storage");

            migrationBuilder.DropColumn(
                name: "TotalVolume",
                table: "Container");

            migrationBuilder.DropColumn(
                name: "UtilizationRate",
                table: "Container");

            migrationBuilder.DropColumn(
                name: "LastOptimizedDate",
                table: "Storage");

            migrationBuilder.DropColumn(
                name: "OccupiedContainers",
                table: "Storage");

            migrationBuilder.DropColumn(
                name: "TotalContainers",
                table: "Storage");

            migrationBuilder.DropColumn(
                name: "UsedVolume",
                table: "Storage");

            migrationBuilder.DropColumn(
                name: "AverageWeight",
                table: "ProductType");

            migrationBuilder.DropColumn(
                name: "AvoidSunlight",
                table: "ProductType");

            migrationBuilder.DropColumn(
                name: "CanStack",
                table: "ProductType");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ProductType");

            migrationBuilder.DropColumn(
                name: "IsFragile",
                table: "ProductType");

            migrationBuilder.DropColumn(
                name: "MaxStackLayers",
                table: "ProductType");

            migrationBuilder.DropColumn(
                name: "PlacementPriority",
                table: "ProductType");

            migrationBuilder.DropColumn(
                name: "PreferredZone",
                table: "ProductType");

            migrationBuilder.DropColumn(
                name: "RequireMoistureControl",
                table: "ProductType");

            migrationBuilder.DropColumn(
                name: "RequireVentilation",
                table: "ProductType");

            migrationBuilder.DropColumn(
                name: "ShapeType",
                table: "ProductType");

            migrationBuilder.DropColumn(
                name: "CurrentContainerCount",
                table: "Floor");

            migrationBuilder.DropColumn(
                name: "CurrentWeight",
                table: "Floor");

            migrationBuilder.DropColumn(
                name: "MaxContainers",
                table: "Floor");

            migrationBuilder.DropColumn(
                name: "MaxWeight",
                table: "Floor");

            migrationBuilder.DropColumn(
                name: "PositionX",
                table: "Floor");

            migrationBuilder.DropColumn(
                name: "PositionY",
                table: "Floor");

            migrationBuilder.DropColumn(
                name: "PositionZ",
                table: "Floor");

            migrationBuilder.DropColumn(
                name: "UtilizationRate",
                table: "Floor");

            migrationBuilder.DropColumn(
                name: "Algorithm",
                table: "ContainerLocationLog");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "ContainerLocationLog");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "ContainerLocationLog");

            migrationBuilder.DropColumn(
                name: "CurrentItemCount",
                table: "Container");

            migrationBuilder.DropColumn(
                name: "CurrentWeight",
                table: "Container");

            migrationBuilder.DropColumn(
                name: "HasFragileItems",
                table: "Container");

            migrationBuilder.DropColumn(
                name: "HasHeavyItems",
                table: "Container");

            migrationBuilder.DropColumn(
                name: "LastOptimizedDate",
                table: "Container");

            migrationBuilder.DropColumn(
                name: "MaxItems",
                table: "Container");

            migrationBuilder.DropColumn(
                name: "MaxWeight",
                table: "Container");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Container");

            migrationBuilder.DropColumn(
                name: "OptimizationScore",
                table: "Container");

            migrationBuilder.DropColumn(
                name: "PositionX",
                table: "Container");

            migrationBuilder.DropColumn(
                name: "PositionY",
                table: "Container");

            migrationBuilder.DropColumn(
                name: "PositionZ",
                table: "Container");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Container");

            migrationBuilder.DropColumn(
                name: "ProductTypeID",
                table: "Container");

            migrationBuilder.DropColumn(
                name: "RotationAngle",
                table: "Container");

            migrationBuilder.DropColumn(
                name: "UsedVolume",
                table: "Container");

            migrationBuilder.RenameColumn(
                name: "PerformedBy",
                table: "ContainerLocationLog",
                newName: "Assign");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Customer",
                type: "varchar(500)",
                unicode: false,
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldUnicode: false,
                oldMaxLength: 500,
                oldDefaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Customer",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50,
                oldDefaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Customer",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50,
                oldDefaultValue: "");

            migrationBuilder.CreateTable(
                name: "ContainerType",
                columns: table => new
                {
                    ContainerTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductTypeID = table.Column<int>(type: "int", nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    Volume = table.Column<decimal>(type: "decimal(18,0)", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_ContainerType_ProductTypeID",
                table: "ContainerType",
                column: "ProductTypeID");
        }
    }
}
