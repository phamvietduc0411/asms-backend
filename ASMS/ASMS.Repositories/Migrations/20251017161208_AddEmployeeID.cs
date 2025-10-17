using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASMS.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK__Employee__1F642549B4A50CB4",
                table: "Employee");

            migrationBuilder.RenameTable(
                name: "Employee",
                newName: "Employees");

            migrationBuilder.RenameIndex(
                name: "IX_Employee_EmployeeRoleID",
                table: "Employees",
                newName: "IX_Employees_EmployeeRoleID");

            migrationBuilder.RenameIndex(
                name: "IX_Employee_BuildingId",
                table: "Employees",
                newName: "IX_Employees_BuildingId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Employee__1F642549B4A50CB4",
                table: "Employees",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK__Employee__1F642549B4A50CB4",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Employees");

            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "Employee");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_EmployeeRoleID",
                table: "Employee",
                newName: "IX_Employee_EmployeeRoleID");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_BuildingId",
                table: "Employee",
                newName: "IX_Employee_BuildingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Employee__1F642549B4A50CB4",
                table: "Employee",
                column: "EmployeeCode");
        }
    }
}
