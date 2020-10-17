﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace leave_management.Data.Migrations
{
    public partial class AddedNumberOfDaysToLeaveTypeAndPeriodToLeaveAllocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefaultNumberOfDays",
                table: "LeaveTypes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Period",
                table: "LeaveAllocations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DetailsLeaveTypeViewModel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetailsLeaveTypeViewModel", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetailsLeaveTypeViewModel");

            migrationBuilder.DropColumn(
                name: "DefaultNumberOfDays",
                table: "LeaveTypes");

            migrationBuilder.DropColumn(
                name: "Period",
                table: "LeaveAllocations");
        }
    }
}
