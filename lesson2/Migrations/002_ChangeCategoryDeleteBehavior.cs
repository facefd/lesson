using Microsoft.EntityFrameworkCore.Migrations;

namespace lesson2.Migrations;

public partial class ChangeCategoryDeleteBehavior : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(name: "FK_Tasks_Categories_CategoryId", table: "Tasks");

        migrationBuilder.AddForeignKey(
            name: "FK_Tasks_Categories_CategoryId",
            table: "Tasks",
            column: "CategoryId",
            principalTable: "Categories",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(name: "FK_Tasks_Categories_CategoryId", table: "Tasks");

        migrationBuilder.AddForeignKey(
            name: "FK_Tasks_Categories_CategoryId",
            table: "Tasks",
            column: "CategoryId",
            principalTable: "Categories",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
