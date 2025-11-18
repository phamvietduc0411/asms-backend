using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASMS.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_Customer_CustomerId1",
                table: "RefreshToken");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_Employees_EmployeeId1",
                table: "RefreshToken");

            migrationBuilder.DropIndex(
                name: "IX_RefreshToken_CustomerId1",
                table: "RefreshToken");

            migrationBuilder.DropIndex(
                name: "IX_RefreshToken_EmployeeId1",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "CustomerId1",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "EmployeeId1",
                table: "RefreshToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId1",
                table: "RefreshToken",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId1",
                table: "RefreshToken",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_CustomerId1",
                table: "RefreshToken",
                column: "CustomerId1");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_EmployeeId1",
                table: "RefreshToken",
                column: "EmployeeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_Customer_CustomerId1",
                table: "RefreshToken",
                column: "CustomerId1",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_Employees_EmployeeId1",
                table: "RefreshToken",
                column: "EmployeeId1",
                principalTable: "Employees",
                principalColumn: "Id");
        }
    }
}
