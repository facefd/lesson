using Microsoft.EntityFrameworkCore.Migrations;

namespace lesson2.Migrations;

// Задача 11: Изменение поведения при удалении категории на SET NULL
// EF Core по умолчанию Cascade Delete - удаляет задачи при удалении категории
// Мы хотим SetNull - задачи остаются, CategoryId становится null
public partial class ChangeCategoryDeleteBehavior : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Удаление старого foreign key
        migrationBuilder.DropForeignKey(
            name: "FK_Tasks_Categories_CategoryId",
            table: "Tasks");

        // Пересоздание foreign key с OnDelete(SetNull)
        migrationBuilder.AddForeignKey(
            name: "FK_Tasks_Categories_CategoryId",
            table: "Tasks",
            column: "CategoryId",
            principalTable: "Categories",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull); // ✅ При удалении категории у задач CategoryId станет null
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Откат обратно к Cascade (по умолчанию)
        migrationBuilder.DropForeignKey(
            name: "FK_Tasks_Categories_CategoryId",
            table: "Tasks");

        migrationBuilder.AddForeignKey(
            name: "FK_Tasks_Categories_CategoryId",
            table: "Tasks",
            column: "CategoryId",
            principalTable: "Categories",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade); // ❌ Удалял бы все задачи этой категории
    }
}
