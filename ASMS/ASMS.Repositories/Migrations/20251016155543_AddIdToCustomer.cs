using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASMS.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddIdToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Order__CustomerC__6EF57B66",
                table: "Order");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Customer__066785204D979A27",
                table: "Customer");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Customer",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Customer_CustomerCode",
                table: "Customer",
                column: "CustomerCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customer",
                table: "Customer",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "UQ_Customer_CustomerCode",
                table: "Customer",
                column: "CustomerCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_CustomerCode",
                table: "Order",
                column: "CustomerCode",
                principalTable: "Customer",
                principalColumn: "CustomerCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_CustomerCode",
                table: "Order");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Customer_CustomerCode",
                table: "Customer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customer",
                table: "Customer");

            migrationBuilder.DropIndex(
                name: "UQ_Customer_CustomerCode",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Customer");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Customer__066785204D979A27",
                table: "Customer",
                column: "CustomerCode");

            migrationBuilder.AddForeignKey(
                name: "FK__Order__CustomerC__6EF57B66",
                table: "Order",
                column: "CustomerCode",
                principalTable: "Customer",
                principalColumn: "CustomerCode");
        }
    }
}
