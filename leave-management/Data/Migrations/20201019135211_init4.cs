using Microsoft.EntityFrameworkCore.Migrations;

namespace leave_management.Data.Migrations
{
    public partial class init4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequsetingEmployeeId",
                table: "LeaveRequests");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequsetingEmployeeId",
                table: "LeaveRequests",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
